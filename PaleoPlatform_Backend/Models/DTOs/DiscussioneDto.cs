using System;
using System.Collections.Generic;

namespace PaleoPlatform_Backend.Models.DTOs
{
    public class DiscussioneCreateDto
    {
        public string Titolo { get; set; }
        public string Contenuto { get; set; }
        public int TopicId { get; set; }
    }

    public class DiscussioneReadDto
    {
        public int Id { get; set; }
        public string Titolo { get; set; } = string.Empty;
        public string? Contenuto { get; set; }
        public string AutoreUsername { get; set; } = string.Empty;
        public DateTime DataCreazione { get; set; }

        public int Score { get; set; }  // Upvotes - Downvotes
        public List<CommentoReadDto> Commenti { get; set; }
        public int CommentCount { get; set; }
        public int TopicId { get; set; }


    }

}
