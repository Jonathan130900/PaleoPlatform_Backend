using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models.DTOs
{
    public class RegistrazioneDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
