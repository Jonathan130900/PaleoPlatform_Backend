namespace PaleoPlatform_Backend.Models.DTOs
{
    public class CommentoCreateDto
    {
        public string Contenuto { get; set; }
        public int? ParentCommentId { get; set; }
        public int? ArticoloId { get; set; }
        public int? DiscussioneId { get; set; }
    }

    public class CommentoReadDto
    {
        public int Id { get; set; }
        public string Contenuto { get; set; }
        public string UserName { get; set; } // Include UserName here for displaying purposes
        public DateTime CreatedAt { get; set; }
        public int? ParentCommentId { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
    }

    public class VoteDto
    {
        public int CommentoId { get; set; }
        public bool IsUpvote { get; set; } // true for upvote, false for downvote
    }
}
