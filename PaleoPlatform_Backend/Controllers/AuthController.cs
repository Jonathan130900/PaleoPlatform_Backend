using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Models.DTOs;
using PaleoPlatform_Backend.Services;

namespace PaleoPlatform_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtService _jwtService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            JwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            var token = await _jwtService.GenerateTokenAsync(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrazioneDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Ensure the "Utente" role exists
            if (!await _roleManager.RoleExistsAsync("Utente"))
            {
                await _roleManager.CreateAsync(new ApplicationRole("Utente"));
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "Utente");

            var token = await _jwtService.GenerateTokenAsync(user);
            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Ruoli = await _userManager.GetRolesAsync(user)
                }
            });
        }
    }
}
