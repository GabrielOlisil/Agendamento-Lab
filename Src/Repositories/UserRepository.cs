using Agendamentos.Database;
using Agendamentos.Domain.Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Repositories;

public class UserRepository(AgendamentosDbContext context) :IApplicationRepository<User>
{
    public async Task<User?> AddAsync(User entity)
    {
        context.Users.Add(entity);
        
        if (await context.SaveChangesAsync() > 0)
        {
            return entity;
        }

        return null;
    }

    public async Task<User?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>?> GetAllPaginated(int start, int pick)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<User> UpdateAsync(Guid id, User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Count()
    {
        var count = await context.Users.CountAsync();
        
        return count;

    }
}