using Agendamentos.Services;

namespace Agendamentos.Endpoints;

public class UserEndpoints
{
    public static async Task<IResult> BootstrapUser(UserService userService)
    {
        var createUser = await userService.BootstrapAdminUser();

        if (!createUser.resultado || createUser.user is null)
        {
            return TypedResults.BadRequest("Não foi possível criar o usuário administrador");
        }

        return TypedResults.Created($"/users/{createUser.user.Id}",createUser.user);
        
    }
}