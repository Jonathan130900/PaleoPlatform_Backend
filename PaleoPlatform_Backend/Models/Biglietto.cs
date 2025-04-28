namespace PaleoPlatform_Backend.Models
{
    public class Biglietto
    {
        public int Id { get; set; }
        public string UtenteId { get; set; }  // User ID who bought the ticket
        public int EventoId { get; set; }     // Event ID
        public bool Pagato { get; set; }      // Payment status
        public decimal Prezzo { get; set; }   // Price of the ticket 
        public DateTime DataAcquisto { get; set; } // Purchase date

        public ApplicationUser Utente { get; set; }  // Navigation property for user
        public Evento Evento { get; set; }           // Navigation property for event
        public string StripePaymentIntentId { get; set; }
    }
}
