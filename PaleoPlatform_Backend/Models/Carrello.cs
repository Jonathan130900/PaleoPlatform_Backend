namespace PaleoPlatform_Backend.Models
{
    public class Carrello
    {
        public int Id { get; set; }
        public string UtenteId { get; set; }
        public bool Pagato { get; set; }
        public string StripeCheckoutSessionId { get; set; }
        public ICollection<CarrelloItem> Items { get; set; } = new List<CarrelloItem>();
        public DateTime DataCreazione { get; set; } = DateTime.UtcNow;
        public DateTime? DataPagamento { get; set; }
    }

    public class CarrelloItem
    {
        public int Id { get; set; }
        public int ProdottoId { get; set; }
        public int Quantità { get; set; }

        public string UtenteId { get; set; }

        public int CarrelloId { get; set; }
        public Carrello Carrello { get; set; }

        public Prodotto Prodotto { get; set; } // Navigation to know product info
    }
}
