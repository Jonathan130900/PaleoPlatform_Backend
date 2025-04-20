using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models.DTOs
{
    public class TopicCreateDto
    {
        [Required]
        public string Nome { get; set; }
    }

}
