using ApiNotificacoesPush.Data;
using ApiNotificacoesPush.Entities;
using ApiNotificacoesPush.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class DispositivosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<DispositivosController> _logger;

    public DispositivosController(AppDbContext context, ILogger<DispositivosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarToken([FromBody] RegistroTokenDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.UsuarioId))
                return BadRequest("ID do usuário é obrigatório");

            if (string.IsNullOrWhiteSpace(dto.TokenDispositivo))
                return BadRequest("Token do dispositivo é obrigatório");

            // Verifica se já existe token para este usuário
            var existente = await _context.Dispositivos
                .FirstOrDefaultAsync(d => d.UsuarioId == dto.UsuarioId);

            if (existente != null)
            {
                // Atualiza apenas se o token for diferente
                if (existente.TokenDispositivo != dto.TokenDispositivo)
                {
                    existente.TokenDispositivo = dto.TokenDispositivo;
                    existente.UltimaAtualizacao = DateTime.UtcNow;
                    existente.Plataforma = dto.Plataforma;
                    _context.Update(existente);

                    _logger.LogInformation($"Token atualizado para usuário {dto.UsuarioId}");
                }
            }
            else
            {
                var novoDispositivo = new DispositivoUsuario
                {
                    UsuarioId = dto.UsuarioId,
                    TokenDispositivo = dto.TokenDispositivo,
                    Plataforma = dto.Plataforma,
                    DataRegistro = DateTime.UtcNow,
                    UltimaAtualizacao = DateTime.UtcNow
                };
                _context.Add(novoDispositivo);

                _logger.LogInformation($"Novo token registrado para usuário {dto.UsuarioId}");
            }

            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Message = "Token registrado/atualizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar token");
            return StatusCode(500, new { Success = false, Message = "Erro interno no servidor" });
        }
    }

    [HttpGet("por-usuario/{usuarioId}")]
    public async Task<IActionResult> ObterTokenPorUsuario(string usuarioId)
    {
        var dispositivo = await _context.Dispositivos
            .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);

        if (dispositivo == null)
            return NotFound("Dispositivo não encontrado");

        return Ok(dispositivo);
    }
}