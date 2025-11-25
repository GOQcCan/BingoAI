using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BingoAI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            // Get the authorization header
            var authHeader = Request.Headers.Authorization.ToString();
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("No token provided");
            }

            var idToken = authHeader.Substring("Bearer ".Length);

            try
            {
                // Validate the Google ID token
                var googleClientId = _configuration["Authentication:Google:ClientId"];
                
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);
                
                // Token is valid, log the user info
                _logger.LogInformation($"User authenticated: {payload.Email} ({payload.Name})");

                // Return weather data
                return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray());
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning($"Invalid Google token: {ex.Message}");
                return Unauthorized("Invalid Google token");
            }
        }
    }
}