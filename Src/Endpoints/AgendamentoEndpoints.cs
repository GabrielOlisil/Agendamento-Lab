using Agendamentos.Database;
using Agendamentos.Domain.DTOs;
using Agendamentos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Endpoints;

public class AgendamentoEndpoints()
{
    
        
    public static async Task<IResult> ListAgendamento(AgendamentosDbContext dbContext,
        ILogger<AgendamentoEndpoints> logger)
    {
        logger.LogInformation("Listando Agendamentos");
        return TypedResults.Ok(await dbContext.Agendamentos
            .Include(e => e.Ambiente)
            .Include(e => e.Horario)
            .ToListAsync());
    }

    public static async Task<IResult> CreateAgendamento(AgendamentosDbContext dbContext,
        AgendamentoCreateDto agendamento)
    {
        
        var ambiente = await dbContext.Ambientes.FindAsync(agendamento.AmbienteId);
        var horario = await dbContext.Horarios.FindAsync(agendamento.HorarioId);
        
        if (ambiente is null || horario is null)
            return TypedResults.BadRequest("Ambiente ou horário inválido.");
        
        var novoAgendamento = new Agendamento()
        {
            Ambiente = ambiente,
            Data = agendamento.Data,
            Horario = horario,
            Id = Guid.NewGuid()
        };

        dbContext.Agendamentos.Add(novoAgendamento);

        await dbContext.SaveChangesAsync();


        return TypedResults.Created($"/agendamentos/{novoAgendamento.Id}", novoAgendamento);
    }
}