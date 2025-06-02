using Agendamentos.Database;
using Agendamentos.Domain.DTOs;
using Agendamentos.Domain.DTOs.Agendamento;
using Agendamentos.Domain.DTOs.Calendario;
using Agendamentos.Domain.Models;
using Agendamentos.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Helpers;

public class AgendamentoHelper(AgendamentosDbContext dbContext)
{
    public async Task<CalendarioDiaResponseDto> ObterAgendamentosDia(string slug, DateTime dia)
    {
        var aulas = new CalendarioDiaResponseDto()
        {
            Matutino = [],
            Noturno = [],
            Vespertino = []
        };
        
        var aulasHorario =
            await dbContext.Horarios
                .OrderBy(p => p.Rank)
                .ToArrayAsync();
        


        foreach (var horario in aulasHorario)
        {

            switch (horario.Turno)
            {
                case Turno.Matutino:
                    aulas.Matutino.Add(new AgendamentoLabelResponseDto(){Rank = horario.Rank, Label = horario.Rank.ToString(), Status = false});
                    break;
                case Turno.Verspertino:
                    aulas.Vespertino.Add(new AgendamentoLabelResponseDto(){Rank = horario.Rank, Label = horario.Rank.ToString(), Status = false});

                    break;
                case Turno.Noturno:
                    aulas.Noturno.Add(new AgendamentoLabelResponseDto(){Rank = horario.Rank, Label = horario.Rank.ToString(), Status = false});

                    break;
            }
        }
        

        var agendamentos = await dbContext.Agendamentos
            .Include(e => e.Horario)
            .Include(e => e.Ambiente)
            .Include(p => p.Professor)
            .Where(p => p.Ambiente.Slug == slug)
            .Where(e => e.Data.Date == dia.Date)
            .OrderBy(p => p.Horario.Rank)
            .ToListAsync();

        
       if (agendamentos.Count == 0)
        {
            return aulas;
        }
       
        foreach (var ag in agendamentos)
        {
            
            switch (ag.Horario.Turno)
            {
                case Turno.Matutino:
                    var hor = aulas.Matutino.Find(a => a.Rank == ag.Horario.Rank);

                    if (hor is null)
                    {
                        break;
                    }
                    
                    hor.Label = ag.Professor.Nome;
                    hor.Status = true;
                    
                    break;
                
                case Turno.Verspertino:
                    var horVes = aulas.Vespertino.Find(a => a.Rank == ag.Horario.Rank);

                    if (horVes is null)
                    {
                        break;
                    }
                    
                    horVes.Label = ag.Professor.Nome;
                    horVes.Status = true;
                    break;

                case Turno.Noturno:
                    var horNot = aulas.Vespertino.Find(a => a.Rank == ag.Horario.Rank);

                    if (horNot is null)
                    {
                        break;
                    }
                    
                    horNot.Label = ag.Professor.Nome;
                    horNot.Status = true;
                    break;
                

            }
        }

        return aulas;
    }
}