using Agendamentos.Database;
using Agendamentos.Domain.Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Repositories;

public class AgendamentoRepository(AgendamentosDbContext context) : IApplicationRepository<Agendamento>
{
    public async Task<Agendamento?> AddAsync(Agendamento entity)
    {
        context.Agendamentos.Add(entity);

        if (await context.SaveChangesAsync() > 0)
        {
            return entity;
        }

        return null;
    }

    public async Task<Agendamento?> GetById(Guid id)
    {
        var agendamento = await context.Agendamentos.FindAsync(id);

        return agendamento ?? null;
    }

    public async Task<List<Agendamento>> GetAll()
    {
        List<Agendamento> professores;

        professores = await context.Agendamentos
            .AsNoTracking()
            .Where(p => !p.Deleted)
            .ToListAsync();

        return professores;
    }

    public async Task<List<Agendamento>?> GetAllPaginated(int start, int pick)
    {
        var agendamento = await context.Agendamentos
            .Where(p => !p.Deleted)
            .Skip(start)
            .Take(pick).ToListAsync();

        return agendamento;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var agendamento = await GetById(id);

        if (agendamento is null || agendamento.Deleted == false)
        {
            return false;
        }

        agendamento.Deleted = true;

        context.Agendamentos.Update(agendamento);


        if (await context.SaveChangesAsync() > 0)
        {
            return true;
        }

        return false;    }

    public async Task<Agendamento> UpdateAsync(Guid id, Agendamento entity)
    {
        var agendamento = await GetById(id);

        if (agendamento is null)
        {
            throw new KeyNotFoundException("Professor not found.");
        }

        agendamento.Ambiente = entity.Ambiente;
        agendamento.Data = entity.Data;
        agendamento.Horario = entity.Horario;
        agendamento.Professor = entity.Professor;
        agendamento.Deleted = entity.Deleted;
        
        
        context.Agendamentos.Update(agendamento);
        
        
        await context.SaveChangesAsync();

        return agendamento;    }

    public async Task<int> Count() => await context.Agendamentos.Where(p => !p.Deleted).CountAsync();
}