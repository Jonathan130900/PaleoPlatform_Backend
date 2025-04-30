namespace PaleoPlatform_Backend.Models
{
    public class Biglietto
    {
        public int Id { get; set; }
        public string UtenteId { get; set; }
        public int EventoId { get; set; }
        public bool Pagato { get; set; }
        public decimal Prezzo { get; set; }
        public DateTime DataAcquisto { get; set; }

        public ApplicationUser Utente { get; set; }
        public Evento Evento { get; set; }
        public string StripePaymentIntentId { get; set; }
    }
}
