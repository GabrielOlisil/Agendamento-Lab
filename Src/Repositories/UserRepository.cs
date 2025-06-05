using Agendamentos.Database;
using Agendamentos.Domain.Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Repositories;

public class UserRepository(AgendamentosDbContext context) : IUserRepository<User>
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
        var user = await context.Users.FindAsync(id);

        return user ?? null;    
    }

    public async Task<List<User>> GetAll()
    {
        List<User> user;

        user = await context.Users
            .AsNoTracking()
            .Where(p => !p.Deleted)
            .ToListAsync();

        return user;    
        
    }

    public async Task<List<User>?> GetAllPaginated(int start, int pick)
    {
        var user = await context.Users
            .Where(p => !p.Deleted)
            .Skip(start)
            .Take(pick).ToListAsync();

        return user;    
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await GetById(id);

        if (user is null || user.Deleted == false)
        {
            return false;
        }

        user.Deleted = true;

        context.Users.Update(user);


        if (await context.SaveChangesAsync() > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<User> UpdateAsync(Guid id, User entity)
    {
        var user = await GetById(id);

        if (user is null)
        {
            throw new KeyNotFoundException("Professor not found.");
        }

        user.UserName = entity.UserName;

        if (entity.Professor is not null)
        {
            user.Professor = entity.Professor;
        }
        user.Role = entity.Role;
        
        user.Deleted = entity.Deleted;
        
        
        context.Users.Update(user);
        
        
        await context.SaveChangesAsync();

        return user;    }

    public async Task<int> Count()
    {
        var count = await context.Users.CountAsync();

        return count;

    }

    public async Task<User?> FindByUserNameAsync(string usr)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName == usr);

        return user;
    }
}