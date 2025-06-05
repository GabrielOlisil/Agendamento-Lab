using Agendamentos.Domain.DTOs.Auth;
using Agendamentos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    [HttpPost("bootstrap")]
    public async Task<IActionResult> BootstrapUser(UserService userService)
    {
        var createUser = await userService.BootstrapAdminUser();

        if (!createUser.resultado || createUser.user is null)
        {
            return BadRequest("Não foi possível criar o usuário administrador");
        }

        return Created($"/users/{createUser.user.Id}", createUser.user);

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto userDto)
    {
        var token = await userService.Login(userDto);

        if (token is null)
        {
            return Unauthorized("Usuário ou senha inválidos.");
        }

        return Ok(new { token });

    }
}