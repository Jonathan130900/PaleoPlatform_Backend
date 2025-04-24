using PaleoPlatform_Backend.Models;

public class Evento
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

    public ICollection<Biglietto> Biglietti { get; set; } = new List<Biglietto>();

    public ICollection<EventoPartecipazione> Partecipazioni { get; set; } = new List<EventoPartecipazione>();


}
