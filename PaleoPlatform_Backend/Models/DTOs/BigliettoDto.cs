namespace PaleoPlatform_Backend.Models.DTOs
{
    public class BigliettoCreateDto
    {
        public int EventoId { get; set; } // Event ID
        public decimal Prezzo { get; set; } // Price of the ticket
    }
    public class BigliettoReadDto
    {
        public int Id { get; set; }
        public string UtenteId { get; set; }  // User ID who bought the ticket
        public int EventoId { get; set; } // Event ID
        public bool Pagato { get; set; } // Payment status
        public decimal Prezzo { get; set; } // Price of the ticket
        public DateTime DataAcquisto { get; set; } // Purchase date
    }
     

}
