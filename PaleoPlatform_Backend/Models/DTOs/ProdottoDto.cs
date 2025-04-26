namespace PaleoPlatform_Backend.Models.DTOs
{
    public class ProdottoCreateDto
    {
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
        public int QuantitaDisponibile { get; set; }
        public IFormFile Immagine { get; set; }
    }

    public class ProdottoReadDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
        public int QuantitaDisponibile { get; set; }
        public string ImmagineUrl { get; set; }
    }

}
