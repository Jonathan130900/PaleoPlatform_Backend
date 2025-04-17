using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models.DTOs
{
    public class RegistrazioneConRuoloDto
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        public string Ruolo { get; set; } = "Utente";
    }
}
