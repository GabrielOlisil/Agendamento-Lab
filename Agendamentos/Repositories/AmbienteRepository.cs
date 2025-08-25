using Agendamentos.Database;
using Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Repositories;

public class AmbienteRepository(AgendamentosDbContext context) : IApplicationRepository<Ambiente>
{
    public async Task<Ambiente?> AddAsync(Ambiente entity)
    {
        context.Ambientes.Add(entity);

        if (await context.SaveChangesAsync() > 0)
        {
            return entity;
        }

        return null;
    }

    public async Task<Ambiente?> GetById(Guid id)
    {
        var ambiente = await context.Ambientes.FindAsync(id);

        return ambiente ?? null;
    }

    public async Task<List<Ambiente>> GetAll()
    {
        List<Ambiente> ambientes;

        ambientes = await context.Ambientes
            .AsNoTracking()
            .Where(p => !p.Deleted)
            .ToListAsync();

        return ambientes;
    }

    public async Task<List<Ambiente>?> GetAllPaginated(int start, int pick)
    {
        var ambientes = await context.Ambientes
            .Where(p => !p.Deleted)
            .Skip(start)
            .Take(pick).ToListAsync();

        return ambientes;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var ambiente = await GetById(id);

        if (ambiente is null || ambiente.Deleted == false)
        {
            return false;
        }

        ambiente.Deleted = true;

        context.Ambientes.Update(ambiente);


        if (await context.SaveChangesAsync() > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<Ambiente> UpdateAsync(Guid id, Ambiente entity)
    {
        var ambiente = await GetById(id);

        if (ambiente is null)
        {
            throw new KeyNotFoundException("Professor not found.");
        }

        ambiente.Nome = entity.Nome;
        ambiente.SetSlug(entity.Slug);
        ambiente.Deleted = entity.Deleted;


        context.Ambientes.Update(ambiente);


        await context.SaveChangesAsync();

        return ambiente;
    }

    public async Task<int> Count() => await context.Ambientes.Where(p => !p.Deleted).CountAsync();
}