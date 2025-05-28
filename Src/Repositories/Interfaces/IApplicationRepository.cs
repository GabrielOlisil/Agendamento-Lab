namespace Agendamentos.Repositories.Interfaces;

public interface IApplicationRepository<T> where T: class
{
    Task<T?> AddAsync(T entity);
    Task<T?> GetById(Guid id);
    Task<List<T>> GetAll();
    Task<List<T>?> GetAllPaginated(int start, int pick);
    Task<bool> DeleteAsync(Guid id);
    Task<T> UpdateAsync(Guid id, T entity);
    Task<int> Count();
}