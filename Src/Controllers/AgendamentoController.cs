using Agendamentos.Database;
using Agendamentos.Domain.DTOs.Agendamento;
using Agendamentos.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
public class AgendamentoController(AgendamentosDbContext dbContext, ILogger<AgendamentoController> logger)
    : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> ListAgendamento()
    {
        logger.LogInformation("Listando Agendamentos");
        return Ok(await dbContext.Agendamentos
            .Include(e => e.Ambiente)
            .Include(e => e.Horario)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateAgendamento(AgendamentoCreateDto agendamento)
    {
        var disponibilidade = await dbContext.Agendamentos
            .Where(p => p.Data.Date == agendamento.Data.Date)
            .Where(p => p.Horario.Id == agendamento.HorarioId).CountAsync();

        if (disponibilidade > 0)
        {
            return BadRequest("Já existe um agendamento para este horário.");
        }


        var ambiente = await dbContext.Ambientes.FindAsync(agendamento.AmbienteId);


        var horario = await dbContext.Horarios.FindAsync(agendamento.HorarioId);
        var professor = await dbContext.Professores.FindAsync(agendamento.ProfessorId);

        if (ambiente is null || horario is null || professor is null)
            return BadRequest("Ambiente ou horário inválido.");


        var novoAgendamento = new Agendamento()
        {
            Ambiente = ambiente,
            Data = agendamento.Data,
            Horario = horario,
            Professor = professor,
            Id = Guid.NewGuid()
        };


        dbContext.Agendamentos.Add(novoAgendamento);

        await dbContext.SaveChangesAsync();


        return Created($"/agendamentos/{novoAgendamento.Id}", novoAgendamento);
    }
}