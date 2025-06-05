using Agendamentos.Database;
using Agendamentos.Domain.DTOs;
using Agendamentos.Domain.DTOs.User;
using Agendamentos.Domain.Models;
using Agendamentos.Repositories.Interfaces;
using InvalidOperationException = System.InvalidOperationException;

namespace Agendamentos.Services;

public class ProfessorService(
    AgendamentosDbContext context,
    IApplicationRepository<Professor> professorRepository,
    IUserRepository<User> userRepository,
    UserService userService
)
{
    public async Task<Professor?> CreateNewProfessorAndHisUserAsync(Professor professor)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var professorNovo = await professorRepository.AddAsync(professor);
            if (professorNovo is null)
            {
                throw new InvalidOperationException("Não foi possível criar professor");

            }

            var user = new UserProfessorCreate()
            {
                Professor = professorNovo,
                PassWord = "changeme",
            };

            var userNovo = await userService.CreateNewProfessorUserAsync(user);

            if (userNovo is null)
            {
                throw new InvalidOperationException("Não foi possível criar Usuário");
            }
            await transaction.CommitAsync();
            return professorNovo;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }
}