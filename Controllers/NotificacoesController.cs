using ApiNotificacoesPush.Data;
using ApiNotificacoesPush.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class NotificacoesController : ControllerBase
{
    private readonly ILogger<NotificacoesController> _logger;
    private readonly AppDbContext _context;

    public NotificacoesController(ILogger<NotificacoesController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("enviar-para-usuario")]
    public async Task<IActionResult> EnviarNotificacaoParaUsuario([FromBody] NotificacaoUsuarioDto dto)
    {
        try
        {
            // Busca o token do usuário no banco de dados
            var dispositivo = await _context.Dispositivos
                .FirstOrDefaultAsync(d => d.UsuarioId == dto.UsuarioId);

            if (dispositivo == null)
                return NotFound("Usuário não possui token registrado");

            var message = new Message()
            {
                Token = dispositivo.TokenDispositivo,
                Notification = new Notification()
                {
                    Title = dto.Titulo,
                    Body = dto.Mensagem
                },
                Data = dto.DadosAdicionais ?? new Dictionary<string, string>(),
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "default_channel_id",
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Headers = new Dictionary<string, string> { { "apns-priority", "10" } },
                    Aps = new Aps
                    {
                        Sound = "default",
                        Badge = dto.BadgeCount
                    }
                }
            };

            string responseId = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            _logger.LogInformation($"Notificação enviada para {dto.UsuarioId} - ID: {responseId}");

            return Ok(new
            {
                Success = true,
                Message = "Notificação enviada com sucesso!",
                ResponseId = responseId,
                UsuarioId = dto.UsuarioId
            });
        }
        catch (FirebaseMessagingException ex) when (ex.ErrorCode == ErrorCode.NotFound ||
                                                 ex.ErrorCode == ErrorCode.InvalidArgument)
        {
            // Token inválido - podemos remover do banco
            var dispositivoInvalido = await _context.Dispositivos
                .FirstOrDefaultAsync(d => d.UsuarioId == dto.UsuarioId);

            if (dispositivoInvalido != null)
            {
                _context.Dispositivos.Remove(dispositivoInvalido);
                await _context.SaveChangesAsync();
                _logger.LogWarning($"Token inválido removido para usuário {dto.UsuarioId}");
            }

            return BadRequest(new { Success = false, Message = "Token de dispositivo inválido", Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação");
            return StatusCode(500, new { Success = false, Message = "Erro interno no servidor" });
        }
    }
}