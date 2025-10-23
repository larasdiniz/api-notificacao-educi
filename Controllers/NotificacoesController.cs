using ApiNotificacoesPush.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ApiNotificacoesPush.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacoesController : ControllerBase
    {
        private readonly ILogger<NotificacoesController> _logger;

        public NotificacoesController(ILogger<NotificacoesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarNotificacao([FromBody] NotificacaoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TokenDispositivo))
            {
                _logger.LogWarning("Token do dispositivo não fornecido");
                return BadRequest(new { Success = false, Message = "Token do dispositivo é obrigatório" });
            }

            try
            {
                var message = new Message()
                {
                    Token = dto.TokenDispositivo,
                    Notification = new Notification()
                    {
                        Title = dto.Titulo,
                        Body = dto.Mensagem
                    },
                    Android = new AndroidConfig { Priority = Priority.High },
                    Apns = new ApnsConfig
                    {
                        Headers = new Dictionary<string, string>
                        {
                            { "apns-priority", "10" }
                        }
                    }
                };

                string responseId = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                _logger.LogInformation($"Notificação enviada com sucesso: {responseId}");
                return Ok(new
                {
                    Success = true,
                    Message = "Notificação enviada com sucesso!",
                    ResponseId = responseId
                });
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, "Erro no FCM");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Erro ao enviar notificação",
                    Error = ex.Message,
                    ErrorCode = ex.ErrorCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro inesperado");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Erro interno no servidor",
                    Details = ex.Message
                });
            }
        }
    }
}
