using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PaleoPlatform_Backend.Models
{
    public class EventoPartecipazione
    {
        public int Id { get; set; }

        public string UtenteId { get; set; }
        public ApplicationUser Utente { get; set; }
         
        public int EventoId { get; set; }
        public Evento Evento { get; set; }

        public DateTime DataPartecipazione { get; set; } = DateTime.UtcNow;
    }
}
