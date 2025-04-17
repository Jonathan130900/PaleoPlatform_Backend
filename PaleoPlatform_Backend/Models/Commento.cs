using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models
{
    public class Commento
    {
        public int Id { get; set; }

        [Required]
        public string Contenuto { get; set; }

        public DateTime DataPubblicazione { get; set; } = DateTime.UtcNow;

        // Foreign key for the user who created the comment
        public string UtenteId { get; set; }
        public ApplicationUser Utente { get; set; }

        // For replies: if it's a top-level comment, this will be null
        public int? ParentCommentId { get; set; }
        public Commento ParentComment { get; set; }

        // For upvotes and downvotes
        public int Upvotes { get; set; } = 0;
        public int Downvotes { get; set; } = 0;

        // Comments under articles
        public int ArticoloId { get; set; }  // This is the foreign key to Articoli

        // Navigation property back to Articolo
        public Articolo Articolo { get; set; }

    }
}
