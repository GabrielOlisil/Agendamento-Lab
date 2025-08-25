using Agendamentos.Database;
using Models.DTOs.User;
using Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        const string userPass = "changeme";
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
                PassWord = userPass,
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