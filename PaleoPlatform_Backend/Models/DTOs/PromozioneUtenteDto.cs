using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models.DTOs
{
    public class PromozioneUtenteDto
    {
        [Required]
        public string IdUtente { get; set; }

        [Required]
        public string NuovoRuolo { get; set; }
    }
}
