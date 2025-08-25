using Agendamentos.Database;
using Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Repositories;

public class HorarioRepository(AgendamentosDbContext context) : IApplicationRepository<Horario>
{
    public async Task<Horario?> AddAsync(Horario entity)
    {
        context.Horarios.Add(entity);

        if (await context.SaveChangesAsync() > 0)
        {
            return entity;
        }

        return null;
    }

    public async Task<Horario?> GetById(Guid id)
    {
        var horario = await context.Horarios.FindAsync(id);

        return horario ?? null;
    }

    public async Task<List<Horario>> GetAll()
    {
        List<Horario> horario;

        horario = await context.Horarios
            .AsNoTracking()
            .Where(p => !p.Deleted)
            .ToListAsync();

        return horario;
    }

    public async Task<List<Horario>?> GetAllPaginated(int start, int pick)
    {
        var horarios = await context.Horarios
            .Where(p => !p.Deleted)
            .Skip(start)
            .Take(pick).ToListAsync();

        return horarios;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var horario = await GetById(id);

        if (horario is null || horario.Deleted == false)
        {
            return false;
        }

        horario.Deleted = true;

        context.Horarios.Update(horario);


        if (await context.SaveChangesAsync() > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<Horario> UpdateAsync(Guid id, Horario entity)
    {
        var horario = await GetById(id);

        if (horario is null)
        {
            throw new KeyNotFoundException("Horario not found.");
        }

        horario.Inicio = entity.Inicio;
        horario.Rank = entity.Rank;
        horario.Turno = entity.Turno;
        horario.Deleted = entity.Deleted;
        horario.Deleted = entity.Deleted;

        context.Horarios.Update(horario);


        await context.SaveChangesAsync();

        return horario;
    }

    public async Task<int> Count() => await context.Horarios.Where(p => !p.Deleted).CountAsync();
}