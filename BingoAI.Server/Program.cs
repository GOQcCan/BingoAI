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

            ConfigureServices(builder);
            ConfigureAuthentication(builder);

            var app = builder.Build();

            ConfigurePipeline(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
                    ?? "Data Source=BingoAI.db"));

            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            ConfigureCors(builder);

            builder.Services.AddSingleton<IAuthorizationHandler, ImageOwnerAuthorizationHandler>();
        }

        private static void ConfigureCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    var allowedOrigins = builder.Configuration
                        .GetSection("Cors:AllowedOrigins")
                        .Get<string[]>() ?? [];

                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            var googleClientId = builder.Configuration["Authentication:Google:ClientId"]
                ?? throw new InvalidOperationException("Google ClientId is not configured");
            var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"]
                ?? throw new InvalidOperationException("Facebook AppId is not configured");
            var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]
                ?? throw new InvalidOperationException("Facebook AppSecret is not configured");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Google", options => ConfigureGoogleJwtBearer(options, googleClientId))
            .AddJwtBearer("Facebook", options => ConfigureFacebookJwtBearer(options, facebookAppId, facebookAppSecret));

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Google", "Facebook")
                    .Build();
            });
        }

        private static void ConfigureGoogleJwtBearer(JwtBearerOptions options, string googleClientId)
        {
            options.Authority = "https://accounts.google.com";
            options.Audience = googleClientId;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = ["https://accounts.google.com", "accounts.google.com"],
                ValidateAudience = true,
                ValidAudience = googleClientId,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").LastOrDefault();
                    if (!string.IsNullOrEmpty(token) && IsFacebookToken(token))
                    {
                        context.NoResult();
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Google authentication failed: {Error}", context.Exception.Message);
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Google token validated for user: {Email}", email);
                    }
                    return Task.CompletedTask;
                }
            };
        }

        private static void ConfigureFacebookJwtBearer(JwtBearerOptions options, string facebookAppId, string facebookAppSecret)
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").LastOrDefault();
                    if (string.IsNullOrEmpty(token) || !IsFacebookToken(token))
                    {
                        context.NoResult();
                        return Task.CompletedTask;
                    }
                    return ValidateFacebookTokenAsync(context, facebookAppId, facebookAppSecret);
                }
            };
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                ConfigureDevelopmentEnvironment(app);
            }
            else
            {
                ConfigureProductionEnvironment(app);
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("AllowAngularApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }

        private static void ConfigureDevelopmentEnvironment(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        private static void ConfigureProductionEnvironment(WebApplication app)
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
            app.UseHsts();
        }

        /// <summary>
        /// Détecte si un token est un token d'accès Facebook.
        /// Les tokens Facebook commencent typiquement par "EAA" et ne contiennent pas de points (.)
        /// contrairement aux JWT standard (Header.Payload.Signature).
        /// </summary>
        private static bool IsFacebookToken(string token)
        {
            return token.StartsWith("EAA") && !token.Contains('.');
        }

        private static async Task ValidateFacebookTokenAsync(MessageReceivedContext context, string facebookAppId, string facebookAppSecret)
        {
            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").LastOrDefault();
            if (string.IsNullOrEmpty(token)) return;

            var httpClientFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                var debugResponse = await GetFacebookDebugResponseAsync(httpClient, facebookAppId, facebookAppSecret, token);
                if (debugResponse == null) return;

                var data = debugResponse.RootElement.GetProperty("data");
                if (!IsFacebookTokenValid(data)) return;

                var userId = data.GetProperty("user_id").GetString();
                var userData = await GetFacebookUserDataAsync(httpClient, token);
                if (userData == null) return;

                if (userData is JsonElement userElement)
                {
                    var claims = BuildFacebookClaims(userId, userElement);
                    context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Facebook"));
                    context.Success();

                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Facebook token validated for user: {Email}",
                            userElement.TryGetProperty("email", out var userEmail) ? userEmail.GetString() : "unknown");
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(ex, "Facebook token validation failed: {Error}", ex.Message);
                }
            }
        }

        private static async Task<JsonDocument?> GetFacebookDebugResponseAsync(HttpClient httpClient, string appId, string appSecret, string token)
        {
            var debugUrl = $"https://graph.facebook.com/debug_token?input_token={token}&access_token={appId}|{appSecret}";
            var response = await httpClient.GetAsync(debugUrl);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json);
        }

        private static bool IsFacebookTokenValid(JsonElement data)
        {
            return data.TryGetProperty("is_valid", out var isValidProp) && isValidProp.GetBoolean();
        }

        private static async Task<JsonElement?> GetFacebookUserDataAsync(HttpClient httpClient, string token)
        {
            var userUrl = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={token}";
            var userResponse = await httpClient.GetAsync(userUrl);
            if (!userResponse.IsSuccessStatusCode) return null;
            var userJson = await userResponse.Content.ReadAsStringAsync();
            return JsonDocument.Parse(userJson).RootElement;
        }

        private static List<Claim> BuildFacebookClaims(string? userId, JsonElement userData)
        {
            return new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId ?? ""),
                new(ClaimTypes.Name, userData.TryGetProperty("name", out var name) ? name.GetString() ?? "" : ""),
                new(ClaimTypes.Email, userData.TryGetProperty("email", out var email) ? email.GetString() ?? "" : ""),
                new("provider", "facebook")
            };
        }
    }
}
