namespace PaleoPlatform_Backend.Models.DTOs
{
    public class ArticoloCreateDto
    {
        public string Titolo { get; set; }
        public string Contenuto { get; set; }
        public IFormFile? Copertina { get; set; }
    }

    public class ArticoloReadDto
    {
        public int Id { get; set; }
        public string Titolo { get; set; }
        public string Contenuto { get; set; }
        public string AutoreEmail { get; set; }
        public DateTime DataPubblicazione { get; set; }
    }

    public class InlineImageUploadDto
    {
        public IFormFile File { get; set; }
    }

    public class ArticoloUpdateDto
    {
        public string Titolo { get; set; }
        public string Contenuto { get; set; }
        public IFormFile? Copertina { get; set; }
    }
}
