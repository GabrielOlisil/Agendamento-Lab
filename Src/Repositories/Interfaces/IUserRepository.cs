using Agendamentos.Domain.Models;

namespace Agendamentos.Repositories.Interfaces;

public interface IUserRepository<T> : IApplicationRepository<T> where T : User
{
    Task<T?> FindByUserNameAsync(string usr);
}