using Agendamentos.Database;
using Agendamentos.Domain.Models;
using Agendamentos.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Helpers;

public class AgendamentoHelper(AgendamentosDbContext dbContext)
{
    public async Task<CalendarioDiaResponse> ObterAgendamentosDia(string slug, DateTime dia)
    {
        var aulas = new CalendarioDiaResponse();
        
        var aulasHorario =
            await dbContext.Horarios
                .OrderBy(p => p.Rank)
                .ToArrayAsync();

        
        aulas.Matutino = [];
        aulas.Vespertino = [];
        aulas.Noturno = [];


        foreach (var mat in aulasHorario)
        {

            switch (mat.Turno)
            {
                case Turno.Matutino:
                    aulas.Matutino.Add(new AgendamentoLabelResponse(){Rank = mat.Rank, Label = mat.Rank.ToString(), Status = false});
                    break;
                case Turno.Verspertino:
                    aulas.Vespertino.Add(new AgendamentoLabelResponse(){Rank = mat.Rank, Label = mat.Rank.ToString(), Status = false});

                    break;
                case Turno.Noturno:
                    aulas.Noturno.Add(new AgendamentoLabelResponse(){Rank = mat.Rank, Label = mat.Rank.ToString(), Status = false});

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
                    aulas.Matutino.Find(a => a.Rank == ag.Horario.Rank).Label = ag.Professor.Nome;
                    aulas.Matutino.Find(a => a.Rank == ag.Horario.Rank).Status = true;
                    break;
                
                case Turno.Verspertino:
                    aulas.Vespertino.Find(a => a.Rank == ag.Horario.Rank).Label = ag.Professor.Nome;
                    aulas.Vespertino.Find(a => a.Rank == ag.Horario.Rank).Status = true;
                    break;

                case Turno.Noturno:
                    aulas.Noturno.Find(a => a.Rank == ag.Horario.Rank).Label = ag.Professor.Nome;
                    aulas.Noturno.Find(a => a.Rank == ag.Horario.Rank).Status = true;
                    break;
            }
        }

        return aulas;
    }
}