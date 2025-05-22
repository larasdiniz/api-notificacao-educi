using ApiNotificacoesPush.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ApiNotificacoesPush.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacoesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> EnviarNotificacao([FromBody] NotificacaoDto dto)
        {
            try
            {
                var message = new Message()
                {
                    Token = dto.TokenDispositivo,
                    Notification = new Notification()
                    {
                        Title = dto.Titulo,
                        Body = dto.Mensagem
                    }
                };

                // Envia a notificação via FCM API v1
                string responseId = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                return Ok(new
                {
                    Success = true,
                    Message = "Notificação enviada com sucesso!",
                    ResponseId = responseId
                });
            }
            catch (FirebaseMessagingException ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Erro no FCM: " + ex.Message,
                    Details = ex.ErrorCode
                });
            }
        }
    }
}