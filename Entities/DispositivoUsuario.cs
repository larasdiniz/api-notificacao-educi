using System.ComponentModel.DataAnnotations;

namespace ApiNotificacoesPush.Entities
{
    public class DispositivoUsuario
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UsuarioId { get; set; }

        [Required]
        public string TokenDispositivo { get; set; }

        public string Plataforma { get; set; }
        public DateTime DataRegistro { get; set; } = DateTime.UtcNow;
        public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
