namespace PaleoPlatform_Backend.Models.DTOs
{
    public class EventoCreateDto
    {
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public string Luogo { get; set; }
        public decimal Prezzo { get; set; }
        public int PostiDisponibili { get; set; }

        public IFormFile? Copertina { get; set; }
    }


    public class EventoReadDto 
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public string Luogo { get; set; }
        public decimal Prezzo { get; set; }
        public int PostiDisponibili { get; set; }
        public string? CopertinaUrl { get; set; }
    }


    public class EventoUpdateDto
    {
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        public string Luogo { get; set; }
        public decimal? Prezzo { get; set; }
        public int? PostiDisponibili { get; set; }

        public IFormFile? Copertina { get; set; }
    }
}
