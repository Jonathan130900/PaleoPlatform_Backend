using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models
{
    public class Commento
    {
        public int Id { get; set; }

        [Required]
        public string Contenuto { get; set; }

        public DateTime DataPubblicazione { get; set; } = DateTime.UtcNow;

        public string UtenteId { get; set; }
        public ApplicationUser Utente { get; set; }

        // For replies: if it's a top-level comment, this will be null
        public int? ParentCommentId { get; set; }
        public Commento ParentComment { get; set; }
        public ICollection<Commento> Replies { get; set; } = new List<Commento>();

        public int Upvotes { get; set; } = 0;
        public int Downvotes { get; set; } = 0;

        // Foreign keys for related content (Articolo or Discussione)
        public int? ArticoloId { get; set; }  // This is the foreign key to Articoli
        public Articolo Articolo { get; set; }

        public int? DiscussioneId { get; set; } // FK to Discussione
        public Discussione Discussione { get; set; }
    }
}
