using Microsoft.AspNetCore.Mvc;
using ApiNotificacoesPush.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiNotificacoesPush.Controllers
{
    [ApiController]
    [Route ("api/[Controller]")]
    public class NotificacoesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public NotificacoesController(IHttpClientFactory httpClientFactory) { 
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarNotificacao([FromBody] NotificacaoDto dto) {
            var client = _httpClientFactory.CreateClient();

            var firebaeServerKey = "minhachave";

            var mensagem = new
            {
                to = dto.TokenDispositivo,
                notificao = new
                {
                    title = dto.Titulo,
                    body = dto.Mensagem
                }
            };

            var json = JsonSerializer.Serialize(mensagem);
            var conteudo = new StringContent(json, Encoding.UTF8, "aplication/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + firebaeServerKey);

            var resposta = await client.PostAsync("https://fcm.google.com/fcm/send", conteudo);

            if (resposta.IsSuccessStatusCode)
            {
                return Ok("Notificação enviada com sucesso!");
            }
            else { 
                return StatusCode((int)resposta.StatusCode, "Erro ao enviar notificação.");
            }
        }
    }
}
