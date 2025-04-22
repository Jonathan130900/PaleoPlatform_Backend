using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models.DTOs
{
    public class TopicCreateDto
    {
        [Required]
        public string Nome { get; set; }
    }

    public class TopicReadDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

}
