using System.ComponentModel.DataAnnotations;

namespace ApiNotificacoesPush.Models
{
    public class RegistroTokenDto
    {
        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public string TokenDispositivo { get; set; }

        public string Plataforma { get; set; } 
    }
}
