using Agendamentos.Database;
using Agendamentos.Domain.Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Repositories;

public class ProfessorRepository(AgendamentosDbContext context) : IApplicationRepository<Professor>
{
    public async Task<Professor?> AddAsync(Professor professor)
    {
        context.Professores.Add(professor);
        
        if (await context.SaveChangesAsync() > 0)
        {
            return professor;
        }

        return null;

    }

    public async Task<Professor?> GetById(Guid id)
    {
        var professor = await context.Professores.FindAsync(id);

        return professor ?? null;
    }

    public async Task<List<Professor>> GetAll()
    {
        List<Professor> professores;
        
        professores = await context.Professores
            .AsNoTracking()
            .ToListAsync();

        return professores;
    }

    public async Task<List<Professor>> GetAllPaginated(int start, int pick)
    {
        var professores = await context.Professores.Skip(start).Take(pick).ToListAsync();

        return professores;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Professor> UpdateAsync(Guid id, Professor entity)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Count()
    {
        throw new NotImplementedException();
    }
}