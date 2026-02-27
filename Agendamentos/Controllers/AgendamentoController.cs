using System.Security.Claims;
using Agendamentos.Database;
using Models.DTOs.Agendamento;
using Models;
using Agendamentos.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AgendamentoController(
    AgendamentosDbContext dbContext,
    ILogger<AgendamentoController> logger,
    AgendamentoHelper agendamentoHelper
)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ListAgendamento()
    {
        logger.LogInformation("Listando Agendamentos");
        return Ok(await dbContext.Agendamentos
            .Include(e => e.Ambiente)
            .Include(e => e.Horario)
            .Include(p => p.Professor)
            .Where(p => !p.Deleted)
            .ToListAsync());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAgendamento(Guid id)
    {
        var agendamento = await dbContext.Agendamentos
            .Include(e => e.Ambiente)
            .Include(e => e.Horario)
            .Include(p => p.Professor)
            .FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);

        if (agendamento == null)
        {
            return NotFound();
        }

        return Ok(agendamento);
    }

    [HttpPost("self")]
    [Authorize(Roles = "user")]
    public async Task<IActionResult> SelfCreateAgendamento(SelfAgendamentoCreateDto agendamento)
    {
        var uid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (uid == null)
        {
            return Unauthorized();
        }

        var user = await dbContext.Users
            .Include(e => e.Professor)
            .FirstOrDefaultAsync(p => p.Id.ToString() == uid);

        if (user?.Professor == null)
        {
            return Forbid();
        }

        var profId = user.Professor.Id;

        var disponibilidade = await dbContext.Agendamentos
            .AnyAsync(p => p.Data.Date == agendamento.Data.Date && p.Horario.Rank == agendamento.HorarioRank && !p.Deleted);

        if (disponibilidade)
        {
            return BadRequest("Já existe um agendamento para este horário.");
        }

        var ambiente = await dbContext.Ambientes.FindAsync(agendamento.AmbienteId);
        var horario = await dbContext.Horarios.FirstOrDefaultAsync(h => h.Rank == agendamento.HorarioRank);

        if (ambiente is null || horario is null)
            return BadRequest("Ambiente ou horário inválido.");

        var novoAgendamento = new Agendamento
        {
            Id = Guid.NewGuid(),
            Ambiente = ambiente,
            Data = agendamento.Data,
            Horario = horario,
            Professor = user.Professor
        };

        dbContext.Agendamentos.Add(novoAgendamento);
        await dbContext.SaveChangesAsync();

        return Created($"/agendamento/{novoAgendamento.Id}", novoAgendamento);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateAgendamento(AgendamentoCreateDto agendamento)
    {
        var disponibilidade = await dbContext.Agendamentos
            .AnyAsync(p => p.Data.Date == agendamento.Data.Date && p.Horario.Id == agendamento.HorarioId && !p.Deleted);

        if (disponibilidade)
        {
            return BadRequest("Já existe um agendamento para este horário.");
        }

        var ambiente = await dbContext.Ambientes.FindAsync(agendamento.AmbienteId);
        var horario = await dbContext.Horarios.FindAsync(agendamento.HorarioId);
        var professor = await dbContext.Professores.FindAsync(agendamento.ProfessorId);

        if (ambiente is null || horario is null || professor is null)
            return BadRequest("Ambiente, horário ou professor inválido.");

        var novoAgendamento = new Agendamento
        {
            Id = Guid.NewGuid(),
            Ambiente = ambiente,
            Data = agendamento.Data,
            Horario = horario,
            Professor = professor
        };

        dbContext.Agendamentos.Add(novoAgendamento);
        await dbContext.SaveChangesAsync();

        return Created($"/agendamento/{novoAgendamento.Id}", novoAgendamento);
    }

    [HttpPost("lote/self")]
    [Authorize(Roles = "user")]
    public async Task<IActionResult> SelfCreateAgendamentoEmLote(SelfAgendamentoLoteCreateDto selfDto)
    {
        if (selfDto.SemanasAReplicar <= 0)
        {
            return BadRequest("O número de semanas a replicar deve ser maior que zero.");
        }

        // 1. Identifica o professor a partir do token de autenticação
        var uid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (uid == null) return Unauthorized();

        var user = await dbContext.Users
            .Include(u => u.Professor)
            .FirstOrDefaultAsync(u => u.Id.ToString() == uid);

        if (user?.Professor == null)
        {
            return Forbid("Usuário não está associado a um perfil de professor.");
        }

        // 2. Prepara o DTO completo para a lógica de negócio
        var agendamentoDto = new AgendamentoLoteCreateDto
        {
            ProfessorId = user.Professor.Id,
            AmbienteId = selfDto.AmbienteId,
            DataInicio = selfDto.DataInicio,
            HorarioInicioId = selfDto.HorarioInicioId,
            HorarioFimId = selfDto.HorarioFimId,
            SemanasAReplicar = selfDto.SemanasAReplicar
        };

        // 3. Reutiliza a mesma lógica do helper que o admin usa
        var (sucesso, mensagem) = await agendamentoHelper.CriarAgendamentosEmLote(agendamentoDto);

        if (!sucesso)
        {
            if (mensagem.StartsWith("Conflito"))
            {
                return Conflict(new { message = mensagem });
            }

            return BadRequest(new { message = mensagem });
        }

        return Created("", new { message = mensagem });
    }

    [HttpPost("lote")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateAgendamentoEmLote(AgendamentoLoteCreateDto agendamentoDto)
    {
        if (agendamentoDto.SemanasAReplicar <= 0)
        {
            return BadRequest("O número de semanas a replicar deve ser maior que zero.");
        }

        var (sucesso, mensagem) = await agendamentoHelper.CriarAgendamentosEmLote(agendamentoDto);

        if (!sucesso)
        {
            // Retorna 409 Conflict em caso de conflito de horários
            if (mensagem.StartsWith("Conflito"))
            {
                return Conflict(new { message = mensagem });
            }

            return BadRequest(new { message = mensagem });
        }

        return Created("", new { message = mensagem });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateAgendamento(Guid id, AgendamentoCreateDto agendamentoUpdate)
    {
        var agendamento = await dbContext.Agendamentos.FindAsync(id);
        if (agendamento == null || agendamento.Deleted)
        {
            return NotFound();
        }

        var ambiente = await dbContext.Ambientes.FindAsync(agendamentoUpdate.AmbienteId);
        var horario = await dbContext.Horarios.FindAsync(agendamentoUpdate.HorarioId);
        var professor = await dbContext.Professores.FindAsync(agendamentoUpdate.ProfessorId);

        if (ambiente is null || horario is null || professor is null)
            return BadRequest("Ambiente, horário ou professor inválido.");

        agendamento.Ambiente = ambiente;
        agendamento.Data = agendamentoUpdate.Data;
        agendamento.Horario = horario;
        agendamento.Professor = professor;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PatchAgendamento(Guid id, [FromBody] AgendamentoPatchDto patchDto)
    {
        var agendamento = await dbContext.Agendamentos.FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);
        if (agendamento == null)
        {
            return NotFound();
        }

        if (patchDto.AmbienteId.HasValue)
        {
            var ambiente = await dbContext.Ambientes.FindAsync(patchDto.AmbienteId.Value);
            if (ambiente == null) return BadRequest("Ambiente inválido.");
            agendamento.Ambiente = ambiente;
        }

        if (patchDto.Data.HasValue)
        {
            agendamento.Data = patchDto.Data.Value;
        }

        if (patchDto.HorarioId.HasValue)
        {
            var horario = await dbContext.Horarios.FindAsync(patchDto.HorarioId.Value);
            if (horario == null) return BadRequest("Horário inválido.");
            agendamento.Horario = horario;
        }

        if (patchDto.ProfessorId.HasValue)
        {
            var professor = await dbContext.Professores.FindAsync(patchDto.ProfessorId.Value);
            if (professor == null) return BadRequest("Professor inválido.");
            agendamento.Professor = professor;
        }

        await dbContext.SaveChangesAsync();
        return Ok(agendamento);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteAgendamento(Guid id)
    {
        var agendamento = await dbContext.Agendamentos.FindAsync(id);
        if (agendamento == null)
        {
            return NotFound();
        }

        agendamento.Deleted = true; // Soft delete
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}