using System;
using System.ComponentModel.DataAnnotations;

namespace PaleoPlatform_Backend.Models
{
    public class ExpiredToken
    {
        [Key]
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}