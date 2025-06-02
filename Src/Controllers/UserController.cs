using Agendamentos.Domain.DTOs.Auth;
using Agendamentos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agendamentos.Controllers;

public class UserController(UserService userService): ControllerBase
{
    public  async Task<IActionResult> BootstrapUser(UserService userService)
    {
        var createUser = await userService.BootstrapAdminUser();

        if (!createUser.resultado || createUser.user is null)
        {
            return BadRequest("Não foi possível criar o usuário administrador");
        }

        return Created($"/users/{createUser.user.Id}",createUser.user);
        
    }
    
    public  async Task<IResult> Login( LoginDto userDto)
    {
        return await userService.Login(userDto);

    }
}