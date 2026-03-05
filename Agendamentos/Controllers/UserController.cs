using Models.DTOs.Auth;
using Agendamentos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserService userService, SessionService sessionService, ILogger<UserController> logger) : ControllerBase
{
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        
        if (!Request.Cookies.ContainsKey("SessionToken"))
        {
            return Unauthorized("Não foi possível renovar o token.");

        }

        var refreshToken = Request.Cookies["SessionToken"];
        
        
        logger.LogInformation("Token: {0}",Request.Cookies["SessionToken"]);
        
        
        
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return BadRequest("Refresh token é obrigatório.");
        }
        
        
        var token = await sessionService.RefreshTokenAsync(refreshToken);

        if (!token.HasValue)
        {
            return Unauthorized("Não foi possível renovar o token.");
        }
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Prevents client-side scripts from accessing the cookie
            SameSite = SameSiteMode.Strict, 
            Expires = DateTimeOffset.UtcNow.AddMinutes(30) // Set an appropriate expiration
        };  
        Response.Cookies.Append("SessionToken", token.Value.refreshToken, cookieOptions);

        
        return Ok(new
        {
            accessToken = token.Value.accessToken,
        });
        
    }
    
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

        if (!token.HasValue) return Unauthorized("Usuário ou senha inválidos.");
        
        

        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Prevents client-side scripts from accessing the cookie
            SameSite = SameSiteMode.Strict, 
            Expires = DateTimeOffset.UtcNow.AddMinutes(30) // Set an appropriate expiration
        };
        Response.Cookies.Append("SessionToken", token.Value.refreshToken, cookieOptions);

        return Ok(new { token.Value.token });

    }
    
    [HttpPost("loginprof")]
    public async Task<IActionResult> LoginProfessor(LoginProfesorDto userDto)
    {
        
        
        var login = new LoginDto()
        {
            PassWord = "changeme",
            UserName = userDto.Matricula
        };
        
        var token = await userService.Login(login);

        if (token is null )
        {
            return Unauthorized("Usuário ou senha inválidos.");
        }
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            SameSite = SameSiteMode.Strict, 
            Expires = DateTimeOffset.UtcNow.AddMinutes(30) 
        };  
        Response.Cookies.Append("SessionToken", token.Value.refreshToken, cookieOptions);

        return Ok(new { token.Value.token });

    }
}