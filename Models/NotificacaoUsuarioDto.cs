using System.ComponentModel.DataAnnotations;

namespace ApiNotificacoesPush.Models
{
    public class NotificacaoUsuarioDto
    {
        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Mensagem { get; set; }

        public Dictionary<string, string> DadosAdicionais { get; set; }
        public int BadgeCount { get; set; } = 1;
    }
}
