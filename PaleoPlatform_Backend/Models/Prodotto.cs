namespace PaleoPlatform_Backend.Models
{
    public class Prodotto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
        public int QuantitaDisponibile { get; set; }
        public string ImmagineUrl { get; set; } // Path to the uploaded image
    }

}
