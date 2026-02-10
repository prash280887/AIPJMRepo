using Microsoft.AspNetCore.Mvc;
using WebApiService.Models;
using WebApiService.Services;

namespace WebApiService.Controllers
{
    /// <summary>
    /// Authentication controller for user login and validation.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IJwtService _jwtService;

        public AuthController(ILogger<AuthController> logger, IJwtService jwtService)
        {
            _logger = logger;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Validates user credentials (username and password).
        /// Dummy implementation: accepts username "admin" and password "password".
        /// Replace with actual user database/identity service in production.
        /// </summary>
        /// <param name="request">Login request with username and password</param>
        /// <returns>Login response with validation result and user info</returns>
        [HttpPost("ValidateUser")]
        public IActionResult ValidateUser([FromBody] LoginRequest request)
        {
            _logger.LogInformation($"Validating user: {request?.Username}");

            // Input validation
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new LoginResponse
                {
                    IsValid = false,
                    Message = "Username and password are required."
                });
            }

            // Dummy user validation (replace with real authentication logic)
            // In production, query a user database, validate password hash, etc.
            if (request.Username == "admin@aipjm.com" && request.Password == "password")
            {
                //Fetch this user infor from DB on Sucessfull validation 
                var userinfo = new UserInfo
                {   sid = "c9fa4f3c-1c8d-4c3b-8c4e-91f6f8d5c7d2",
                    given_name = "Administator",
                    family_name = "aipjm",
                    upn = "admin@aipjm.com",
                    tid ="72f988bf-86f1-41af-91ab-2d7cd011db47",
                    unique_name = "admin@aipjm.com",
                    scp = ["User.Read profile openid email"]
                };

                var jwtToken = _jwtService.GenerateToken(userinfo);
                var response = new LoginResponse
                {
                    IsValid = true,
                    Message = "Login successful.",
                    Token = jwtToken,
                    User = userinfo
                };

                _logger.LogInformation("User authenticated successfully and JWT token generated.");
                return Ok(response);
            }

            // Invalid credentials
            var failResponse = new LoginResponse
            {
                IsValid = false,
                Message = "Invalid username or password."
            };

            _logger.LogWarning($"Failed login attempt for user: {request.Username}");
            return Ok(failResponse); // Return 200 OK with IsValid=false (or use 401 Unauthorized)
        }
    }
}
