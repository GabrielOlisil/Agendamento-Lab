using Agendamentos.Database;
using Models;
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
            .Where(p => !p.Deleted)
            .ToListAsync();

        return professores;
    }

    public async Task<List<Professor>?> GetAllPaginated(int start, int pick)
    {
        var professores = await context.Professores
            .Where(p => !p.Deleted)
            .Skip(start)
            .Take(pick).ToListAsync();

        return professores;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var professor = await GetById(id);

        if (professor is null || professor.Deleted == false)
        {
            return false;
        }

        professor.Deleted = true;

        context.Professores.Update(professor);


        if (await context.SaveChangesAsync() > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<Professor> UpdateAsync(Guid id, Professor entity)
    {
        var professor = await GetById(id);

        if (professor is null)
        {
            throw new KeyNotFoundException("Professor not found.");
        }

        professor.Nome = entity.Nome;
        professor.Matricula = entity.Matricula;
        professor.Slug = entity.Slug;
        professor.Deleted = entity.Deleted;
        context.Professores.Update(professor);
        
        
        await context.SaveChangesAsync();

        return professor;
    }

    public async Task<int> Count() => await context.Professores.Where(p => !p.Deleted).CountAsync();
    
}