using Agendamentos.Database;
using Agendamentos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Endpoints;

public record CalendarioDiaResponse()
{
    public AgendamentoLabelResponse[] Matutino { get; set; }
    public AgendamentoLabelResponse[] Vespertino { get; set; }
    public AgendamentoLabelResponse[] Noturno { get; set; }
}

public record AgendamentoLabelResponse()
{
    public string Label { get; set; } = string.Empty;
    public bool Status { get; set; } = false;
}

public class CalendarioEndpoints
{
    public static async Task<IResult> AgendamentosDia(DateTime dia, AgendamentosDbContext dbContext)
    {

        var totalagendamentosMatutino = await dbContext.Horarios.Where(p => p.Turno == Turno.Matutino).CountAsync();
        var totalagendamentosVespertino = await dbContext.Horarios.Where(p => p.Turno == Turno.Verspertino).CountAsync();
        var totalagendamentosNoturno = await dbContext.Horarios.Where(p => p.Turno == Turno.Noturno).CountAsync();
        
        
        var agendamentos = await dbContext.Agendamentos
            .Include(e => e.Horario)
            .Include(e => e.Ambiente)
            .Where(e => e.Data.Date == dia.Date)
            .ToListAsync();

        var aulasMatutino = Enumerable.Range(0, totalagendamentosMatutino)
            .Select(_ => new AgendamentoLabelResponse())
            .ToArray();
        var aulasVespertino = Enumerable.Range(0, totalagendamentosVespertino)
            .Select(_ => new AgendamentoLabelResponse())
            .ToArray();
        var aulasNoturno = Enumerable.Range(0, totalagendamentosNoturno)
            .Select(_ => new AgendamentoLabelResponse())
            .ToArray();

        
        var aulas = new CalendarioDiaResponse
        {
            Matutino = aulasMatutino,
            Vespertino = aulasVespertino,
            Noturno = aulasNoturno
        };

        if (agendamentos.Count == 0)
        {
            return TypedResults.Ok(aulas);
        }


        foreach (var ag in agendamentos)
        {

            switch (ag.Horario.Turno)
            {
                case Turno.Matutino:
                    aulasMatutino[ag.Horario.Rank -1].Label = "Professor Tal";
                    aulasMatutino[ag.Horario.Rank -1].Status = true;
                    break;
                case Turno.Verspertino:
                    aulasVespertino[ag.Horario.Rank -1].Label = "Professor Tal";
                    aulasVespertino[ag.Horario.Rank -1].Status = true;
                    break;
                
                case Turno.Noturno:
                    aulasNoturno[ag.Horario.Rank -1].Label = "Professor Tal";
                    aulasNoturno[ag.Horario.Rank -1].Status = true;

                    break;
            }
            
        }
        
        
        

        return TypedResults.Ok(aulas);
    }
}