using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fundo.Applications.WebApi.DTOs;
using Fundo.Applications.WebApi.Services;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <param name="request">Login credentials</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid login request for username: {Username}", request.Username);
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Login attempt for username: {Username}", request.Username);

                var response = await _authService.LoginAsync(request);
                if (response == null)
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                    return Unauthorized("Invalid username or password");
                }

                _logger.LogInformation("Successful login for user: {UserId} ({Username})", 
                    response.Id, response.Username);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for username: {Username}", request.Username);
                return StatusCode(500, "An error occurred during login");
            }
        }

        /// <param name="request">Registration details</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid registration request for username: {Username}", request.Username);
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Registration attempt for username: {Username}, email: {Email}", 
                    request.Username, request.Email);

                var response = await _authService.RegisterAsync(request);
                if (response == null)
                {
                    _logger.LogWarning("Failed registration attempt - user already exists: {Username} or {Email}", 
                        request.Username, request.Email);
                    return Conflict("Username or email already exists");
                }

                _logger.LogInformation("Successful registration for user: {UserId} ({Username})", 
                    response.Id, response.Username);

                return CreatedAtAction(nameof(Login), new { username = response.Username }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for username: {Username}", request.Username);
                return StatusCode(500, "An error occurred during registration");
            }
        }

        /// <returns>Current user details</returns>
        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public ActionResult<object> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                var firstName = User.FindFirst("FirstName")?.Value;
                var lastName = User.FindFirst("LastName")?.Value;

                _logger.LogInformation("Current user info requested by user: {UserId} ({Username})", userId, username);

                return Ok(new
                {
                    Id = userId,
                    Username = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Role = role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting current user info");
                return StatusCode(500, "An error occurred while retrieving user information");
            }
        }
    }
} 