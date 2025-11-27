using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BingoAI.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
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
                                ClockSkew = TimeSpan.FromMinutes(5) // Tolérance pour la différence d'horloge
                            };
        
                            // ✅ Logging pour debugging
                            options.Events = new JwtBearerEvents
                            {
                                OnAuthenticationFailed = context =>
                                {
                                    context.HttpContext.RequestServices
                                        .GetRequiredService<ILogger<Program>>()
                                        .LogWarning("Authentication failed: {Error}", context.Exception.Message);
                                    return Task.CompletedTask;
                                },
                                OnTokenValidated = context =>
                                {
                                    var logger = context.HttpContext.RequestServices
                                        .GetRequiredService<ILogger<Program>>();
                                    var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                                    logger.LogInformation("Token validated for user: {Email}", email);
                                    return Task.CompletedTask;
                                }
                            };
                        });

            var app = builder.Build();

            // Disable SPA proxy in development (launch Angular manually)
            // app.UseDefaultFiles();
            // app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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
