using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models
{
    public class Discussione
    {
        public int Id { get; set; }

        [Required]
        public string Titolo { get; set; }

        public string Contenuto { get; set; }

        public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public string AutoreId { get; set; }
        public ApplicationUser Autore { get; set; }

        public int TopicId { get; set; }
        public Topics Topic { get; set; }

        public int Upvotes { get; set; } = 0;
        public int Downvotes { get; set; } = 0;

        public ICollection<Commento> Commenti { get; set; }

    }

}
