using BingoAI.Server.Authorization;
using BingoAI.Server.Data;
using BingoAI.Server.Services;
using BingoAI.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

namespace BingoAI.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpClient(); // Pour valider les tokens Facebook

            // ✅ Configuration SQLite avec Entity Framework Core
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
                    ?? "Data Source=BingoAI.db"));

            // ✅ Enregistrement des services (Dependency Injection - SOLID DIP)
            builder.Services.AddScoped<IImageService, ImageService>();
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS policy for Angular app
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy =>
                    {
                        // ✅ Lire depuis configuration
                        var allowedOrigins = builder.Configuration
                            .GetSection("Cors:AllowedOrigins")
                            .Get<string[]>() ?? [];

                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            var googleClientId = builder.Configuration["Authentication:Google:ClientId"]
                    ?? throw new InvalidOperationException("Google ClientId is not configured");
            var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"]
                    ?? throw new InvalidOperationException("Facebook AppId is not configured");
            var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]
                    ?? throw new InvalidOperationException("Facebook AppSecret is not configured");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer("Google", options =>
                        {
                            options.Authority = "https://accounts.google.com";
                            options.Audience = googleClientId;
        
                            // ✅ Configuration complète pour Google OAuth
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuers = ["https://accounts.google.com", "accounts.google.com"],
                                ValidateAudience = true,
                                ValidAudience = googleClientId,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.FromMinutes(5)
                            };
        
                            // ✅ Logging pour debugging
                            options.Events = new JwtBearerEvents
                            {
                                OnAuthenticationFailed = context =>
                                {
                                    context.HttpContext.RequestServices
                                        .GetRequiredService<ILogger<Program>>()
                                        .LogWarning("Google authentication failed: {Error}", context.Exception.Message);
                                    return Task.CompletedTask;
                                },
                                OnTokenValidated = context =>
                                {
                                    var logger = context.HttpContext.RequestServices
                                        .GetRequiredService<ILogger<Program>>();
                                    var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
                                    logger.LogInformation("Google token validated for user: {Email}", email);
                                    return Task.CompletedTask;
                                }
                            };
                        })
                        .AddJwtBearer("Facebook", options =>
                        {
                            // Facebook n'utilise pas de JWT standard, on valide via Graph API
                            options.Events = new JwtBearerEvents
                            {
                                OnMessageReceived = async context =>
                                {
                                    var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
                                    if (string.IsNullOrEmpty(token)) return;

                                    // Skip si c'est un token Google (commence par ey et contient accounts.google.com)
                                    if (token.StartsWith("ey") && token.Split('.').Length == 3)
                                    {
                                        try
                                        {
                                            var payload = token.Split('.')[1];
                                            var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                                            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(padded));
                                            if (decoded.Contains("accounts.google.com"))
                                            {
                                                return; // C'est un token Google, laisser le schéma Google le gérer
                                            }
                                        }
                                        catch { }
                                    }

                                    var httpClientFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
                                    var httpClient = httpClientFactory.CreateClient();
                                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                                    // Valider le token Facebook via Graph API
                                    var debugUrl = $"https://graph.facebook.com/debug_token?input_token={token}&access_token={facebookAppId}|{facebookAppSecret}";
                                    
                                    try
                                    {
                                        var response = await httpClient.GetAsync(debugUrl);
                                        if (response.IsSuccessStatusCode)
                                        {
                                            var json = await response.Content.ReadAsStringAsync();
                                            var debugResponse = JsonDocument.Parse(json);
                                            var data = debugResponse.RootElement.GetProperty("data");
                                            
                                            if (data.GetProperty("is_valid").GetBoolean())
                                            {
                                                var userId = data.GetProperty("user_id").GetString();
                                                
                                                // Récupérer les infos utilisateur
                                                var userUrl = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={token}";
                                                var userResponse = await httpClient.GetAsync(userUrl);
                                                
                                                if (userResponse.IsSuccessStatusCode)
                                                {
                                                    var userJson = await userResponse.Content.ReadAsStringAsync();
                                                    var userData = JsonDocument.Parse(userJson).RootElement;
                                                    
                                                    var claims = new List<Claim>
                                                    {
                                                        new(ClaimTypes.NameIdentifier, userId ?? ""),
                                                        new(ClaimTypes.Name, userData.TryGetProperty("name", out var name) ? name.GetString() ?? "" : ""),
                                                        new(ClaimTypes.Email, userData.TryGetProperty("email", out var email) ? email.GetString() ?? "" : ""),
                                                        new("provider", "facebook")
                                                    };
                                                    
                                                    context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Facebook"));
                                                    context.Success();
                                                    
                                                    logger.LogInformation("Facebook token validated for user: {Email}", 
                                                        userData.TryGetProperty("email", out var userEmail) ? userEmail.GetString() : "unknown");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogWarning("Facebook token validation failed: {Error}", ex.Message);
                                    }
                                }
                            };
                        });

            // ✅ Policy d'autorisation qui accepte Google OU Facebook
            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Google", "Facebook")
                    .Build();
            });

            // Ajoutez cette ligne après les autres services
            builder.Services.AddSingleton<IAuthorizationHandler, ImageOwnerAuthorizationHandler>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseCors("AllowAngularApp");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Disable fallback to index.html when Angular runs separately
            //app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
