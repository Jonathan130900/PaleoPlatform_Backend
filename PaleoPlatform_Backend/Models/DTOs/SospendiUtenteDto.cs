namespace PaleoPlatform_Backend.Models.DTOs
{
    public class SospendiUtenteDto
    {
        public string UtenteId { get; set; }
        public UserStatus NewStatus { get; set; } // Active, Suspended, Banned
    }
}
