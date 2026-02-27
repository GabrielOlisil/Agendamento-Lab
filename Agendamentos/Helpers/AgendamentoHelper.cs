using Agendamentos.Database;
using Models.DTOs.Agendamento;
using Models.DTOs.Calendario;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Helpers;

public class AgendamentoHelper(AgendamentosDbContext dbContext)
{
    public async Task<(bool Sucesso, string Mensagem)> CriarAgendamentosEmLote(AgendamentoLoteCreateDto dto)
    {
        var ambiente = await dbContext.Ambientes.FindAsync(dto.AmbienteId);
        var professor = await dbContext.Professores.FindAsync(dto.ProfessorId);
        if (ambiente == null || professor == null)
        {
            return (false, "Ambiente ou Professor inválido.");
        }

        var todosHorarios = await dbContext.Horarios.OrderBy(p => p.Rank).ToListAsync();
        var horarioInicio = todosHorarios.FirstOrDefault(h => h.Id == dto.HorarioInicioId);
        var horarioFim = todosHorarios.FirstOrDefault(h => h.Id == dto.HorarioFimId);

        if (horarioInicio == null || horarioFim == null || horarioInicio.Rank > horarioFim.Rank)
        {
            return (false, "Intervalo de horários inválido.");
        }

        var horariosParaAgendar = todosHorarios
            .Where(h => h.Rank >= horarioInicio.Rank && h.Rank <= horarioFim.Rank)
            .ToList();

        var agendamentosPotenciais = new List<Agendamento>();


        for (int i = 0; i <= dto.SemanasAReplicar; i++)
        {
            var dataDaSemana = dto.DataInicio.AddDays(i * 7);
            foreach (var horario in horariosParaAgendar)
            {
                var conflito = await dbContext.Agendamentos
                    .AnyAsync(a =>
                        a.Data == dataDaSemana
                        && a.Horario.Rank == horario.Rank
                        && a.Ambiente.Id == dto.AmbienteId
                    );

                if (conflito)
                {
                    return (false, $"O horário {horario.Rank} do dia {dataDaSemana.Date} no laboratório {ambiente.Nome} já está ocupado");
                }
                

                agendamentosPotenciais.Add(new Agendamento
                {
                    Id = Guid.NewGuid(),
                    Ambiente = ambiente,
                    Professor = professor,
                    Data = dataDaSemana.Date,
                    Horario = horario
                });
            }
        }


        
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Agendamentos.AddRangeAsync(agendamentosPotenciais);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, "Ocorreu um erro ao salvar os agendamentos.");
        }

        return (true, $"{agendamentosPotenciais.Count} agendamentos criados com sucesso.");
    }

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
                    aulas.Matutino.Add(new AgendamentoLabelResponseDto()
                        { Rank = horario.Rank, Inicio = horario.Inicio.ToString(), Ocupado = false });
                    break;
                case Turno.Verspertino:
                    aulas.Vespertino.Add(new AgendamentoLabelResponseDto()
                        { Rank = horario.Rank, Inicio = horario.Inicio.ToString() , Ocupado = false });

                    break;
                case Turno.Noturno:
                    aulas.Noturno.Add(new AgendamentoLabelResponseDto()
                        { Rank = horario.Rank, Inicio = horario.Inicio.ToString(), Ocupado = false });

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
                    hor.Ocupado = true;

                    break;

                case Turno.Verspertino:
                    var horVes = aulas.Vespertino.Find(a => a.Rank == ag.Horario.Rank);

                    if (horVes is null)
                    {
                        break;
                    }

                    horVes.Label = ag.Professor.Nome;
                    horVes.Ocupado = true;
                    break;

                case Turno.Noturno:
                    var horNot = aulas.Vespertino.Find(a => a.Rank == ag.Horario.Rank);

                    if (horNot is null)
                    {
                        break;
                    }

                    horNot.Label = ag.Professor.Nome;
                    horNot.Ocupado = true;
                    break;
            }
        }

        return aulas;
    }
}