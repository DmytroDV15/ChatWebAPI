using ChatWebAPI.Models;
using ChatWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using ChatWebAPI.Repository;
using ChatWebAPI.Services;

namespace ChatWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly TokenProvider tokenProvaider;
        private readonly IRepository<RegistrationModel> _userRepository;

        public AccountController(IAuthService authService, TokenProvider tokenProvaider, IRepository<RegistrationModel> userRepository)
        {
            _authService = authService;
            this.tokenProvaider = tokenProvaider;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { title = "Validation Error", errors = ModelState });
            }

            try
            {
                await _authService.RegisterUserAsync(registrationDto.UserName, registrationDto.Email, registrationDto.Password);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { title = "Registration Error", detail = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RegistrationModel>> Login([FromBody] LoginModel loginDto)
        {
            try
            {
                var user = await _authService.AuthenticateUserAsync(loginDto.Email, loginDto.Password);
                if (user == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                var token = tokenProvaider.Create(user);

                return Ok(new { Message = "Login successful", User = user, Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { title = "Login Error", detail = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("checkAuth")]
        public async Task<IActionResult> CheckAuth()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (email == null || userId == null)
            {
                return Unauthorized(new { Message = "Invalid token or missing claims." });
            }

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(new { Message = "Authenticated", userName = user.UserName, userId = userId });
        }

    }
}
