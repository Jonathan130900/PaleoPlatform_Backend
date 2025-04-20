using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models
{
    public class Topics
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

    }
}
