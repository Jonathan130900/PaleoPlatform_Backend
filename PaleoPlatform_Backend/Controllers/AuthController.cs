﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaleoPlatform_Backend.Data;
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
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IUtenteService _utenteService;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IUtenteService utenteService,
            JwtService jwtService,
            ApplicationDbContext context,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
            _utenteService = utenteService;
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password) || await _userManager.IsInRoleAsync(user, "System"))
                return Unauthorized("Credenziali invalide");

            var token = await _jwtService.GenerateTokenAsync(user);
            return Ok(new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // 1. Get token safely
                var token = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return BadRequest("Invalid token format");
                }
                token = token.Replace("Bearer ", "");

                // 2. Get user safely
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return BadRequest("User not found");

                // 3. Update user last logout
                user.LastLogoutDate = DateTime.UtcNow;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("User update failed: {Errors}", updateResult.Errors);
                }

                // 4. Invalidate token with duplicate check
                if (!await _context.ExpiredTokens.AnyAsync(t => t.Token == token))
                {
                    _context.ExpiredTokens.Add(new ExpiredToken
                    {
                        Token = token,
                        ExpirationDate = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private async Task InvalidateJwtToken(string token)
        {
            // Add to database blacklist
            var expiredToken = new ExpiredToken
            {
                Token = token,
                ExpirationDate = DateTime.UtcNow
            };

            _context.ExpiredTokens.Add(expiredToken);
            await _context.SaveChangesAsync();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrazioneDto dto)
        {
            // Check if email already exists
            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null)
                return BadRequest("Email già in uso.");

            // Check if username already exists
            var existingUsername = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == dto.Username);
            if (existingUsername != null)
                return BadRequest("Username già in uso.");

            // Protect special usernames
            var normalizedUsername = dto.Username.Trim().ToLower();
            if (normalizedUsername == "deleted_user" || normalizedUsername == "banned_user")
                return BadRequest("Username non disponibile.");

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

            if (!await _roleManager.RoleExistsAsync("Utente"))
            {
                await _roleManager.CreateAsync(new ApplicationRole("Utente"));
            }

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

        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            return Ok(new
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Username = User.FindFirstValue(ClaimTypes.Name),
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            });
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var token = await _jwtService.GenerateTokenAsync(user);
            return Ok(new { token });
        }

        [HttpPost("register-with-role")]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> RegistraConRuolo(RegistrazioneConRuoloDto dto)
        {
            var nuovoUtente = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(nuovoUtente, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (!await _roleManager.RoleExistsAsync(dto.Ruolo))
                return BadRequest("Ruolo non valido.");

            await _userManager.AddToRoleAsync(nuovoUtente, dto.Ruolo);

            return Ok(new
            {
                nuovoUtente.Id,
                nuovoUtente.UserName,
                nuovoUtente.Email,
                Ruoli = await _userManager.GetRolesAsync(nuovoUtente)
            });
        }

        [HttpPost("promuovi")]
        [Authorize(Roles = "Amministratore,Moderatore")]
        public async Task<IActionResult> PromuoviUtente(PromozioneUtenteDto dto)
        {
            var utente = await _userManager.FindByIdAsync(dto.IdUtente);
            if (utente == null)
                return NotFound("Utente non trovato.");

            var promotore = await _userManager.GetUserAsync(User);
            var ruoliPromotore = await _userManager.GetRolesAsync(promotore);

            if (dto.NuovoRuolo == "Amministratore" && !ruoliPromotore.Contains("Amministratore"))
                return Forbid("Solo l'amministratore può assegnare il ruolo di Amministratore.");

            if (!await _roleManager.RoleExistsAsync(dto.NuovoRuolo))
                return BadRequest("Ruolo non valido.");

            var ruoliAttuali = await _userManager.GetRolesAsync(utente);
            await _userManager.RemoveFromRolesAsync(utente, ruoliAttuali);
            await _userManager.AddToRoleAsync(utente, dto.NuovoRuolo);

            return Ok($"Utente promosso a {dto.NuovoRuolo}");
        }

        [Authorize]
        [HttpPut("modifica")]
        public async Task<IActionResult> ModificaCredenziali([FromBody] UtenteInfoUpdate dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var utente = await _userManager.FindByIdAsync(userId);
            if (utente == null) return NotFound("Utente non trovato");

            if (!string.IsNullOrEmpty(dto.NuovoUsername))
            {
                var usernameEsistente = await _userManager.Users
                    .AnyAsync(u => u.UserName == dto.NuovoUsername && u.Id != utente.Id);
                if (usernameEsistente)
                    return BadRequest("Username già in uso.");

                utente.UserName = dto.NuovoUsername;
                utente.NormalizedUserName = dto.NuovoUsername.ToUpper();
            }

            if (!string.IsNullOrEmpty(dto.NuovaPassword))
            {
                var newHashed = _passwordHasher.HashPassword(utente, dto.NuovaPassword);
                utente.PasswordHash = newHashed;
            }

            var result = await _userManager.UpdateAsync(utente);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Dati aggiornati con successo.");
        }

        [HttpDelete("delete/{userId}")]
        [Authorize(Roles = "Amministratore")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var requestingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _utenteService.DeleteUserAsync(userId, requestingUserId);

            if (!success)
                return BadRequest("Impossibile eliminare l'utente.");

            return Ok("Utente eliminato con successo.");
        }

        [Authorize]
        [HttpGet("me/claims")]
        public IActionResult GetMyClaims()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity?.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }

        [Authorize(Roles = "Amministratore,Moderatore")]
        [HttpPost("banna-utente")]
        public async Task<IActionResult> BannaUtente(SospendiUtenteDto dto)
        {
            var success = await _utenteService.BanUserAsync(dto.UtenteId, User.FindFirst("id")?.Value);

            if (!success)
                return BadRequest("Impossibile bannare l'utente.");

            return Ok("Utente bannato con successo.");
        }

        [Authorize(Roles = "Amministratore,Moderatore")]
        [HttpPost("riattiva-utente")]
        public async Task<IActionResult> RiattivaUtente(SospendiUtenteDto dto)
        {
            var success = await _utenteService.ReactivateUserAsync(dto.UtenteId, User.FindFirst("id")?.Value);

            if (!success)
                return BadRequest("Impossibile riattivare l'utente.");

            return Ok("Utente riattivato con successo.");
        }

        [Authorize(Roles = "Amministratore,Moderatore")]
        [HttpPost("sospendi-utente")]
        public async Task<IActionResult> SospendiUtente(SospendiUtenteDto dto)
        {
            var success = await _utenteService.SuspendUserAsync(dto.UtenteId, User.FindFirst("id")?.Value);

            if (!success)
                return BadRequest("Impossibile sospendere l'utente.");

            return Ok("Utente sospeso con successo.");
        }


        [Authorize(Roles = "Amministratore")]
        [HttpGet("admin-users")]
        public async Task<IActionResult> GetAdminUsers()
        {
            var users = await _userManager.Users.ToListAsync();  // Fetch all users first

            // Filter the users with the role check being awaited inside a loop or LINQ method.
            var filteredUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                // Check if the user is not 'deleted_user' and does not have the 'System' role
                if (!user.UserName.Equals("deleted_user") &&
                    !await _userManager.IsInRoleAsync(user, "System"))
                {
                    filteredUsers.Add(user);
                }
            }

            return Ok(filteredUsers);
        }


    }
}
