namespace PaleoPlatform_Backend.Models.DTOs
{
    public class CarrelloItemDto
    {
        public int ProdottoId { get; set; }
        public int Quantità { get; set; }
    }

    public class CarrelloReadDto
    {
        public int Id { get; set; }
        public List<CarrelloItemReadDto> Items { get; set; }
    }

    public class CarrelloItemReadDto
    {
        public int ProdottoId { get; set; }
        public string NomeProdotto { get; set; }
        public decimal Prezzo { get; set; }
        public int Quantità { get; set; }
    }
}
