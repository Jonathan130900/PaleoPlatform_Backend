using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models
{
    public class Articolo
    {
        public int Id { get; set; }

        [Required]
        public string Titolo { get; set; }

        [Required]
        public string Contenuto { get; set; }

        public DateTime DataPubblicazione { get; set; } = DateTime.UtcNow;

        [Required]
        public string AutoreId { get; set; }

        public ApplicationUser Autore { get; set; }

        public string? CopertinaUrl { get; set; } = string.Empty;
        public DateTime? DataUltimaModifica { get; set; }


    }
}
