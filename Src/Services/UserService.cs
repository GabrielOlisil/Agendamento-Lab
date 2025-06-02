using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Agendamentos.Domain.DTOs;
using Agendamentos.Domain.DTOs.Auth;
using Agendamentos.Domain.DTOs.User;
using Agendamentos.Domain.Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Agendamentos.Services;

public class UserService(
    IUserRepository<User> userRepository,
    IPasswordHasher<User> hasher,
    IConfiguration configuration
    )
{
    
    public string GenerateToken(string username, string role)
    {
        var key = Encoding.UTF8.GetBytes("iqpevJwhnhFpuAqW0uSRliSrDNpXKEUm2Y1Qc0fq/yY=");
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "http://localhost:8080",
            audience: "http://localhost:8080",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<User?> CreateNewUserAsync(UserCreateDto userDto)
    {
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Role = "admin", 
            UserName = userDto.UserName,
        };

        newUser.PassWordHash = hasher.HashPassword(newUser, userDto.PassWord);

        return await userRepository.AddAsync(newUser);
    }
    
    public async Task<User?> CreateNewProfessorUserAsync(UserProfessorCreate userDto)
    {
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Role = "user",
            Professor = userDto.Professor,
            UserName = SlugService.Generate(userDto.Professor.Nome)
        };

        newUser.PassWordHash = hasher.HashPassword(newUser, userDto.PassWord);

        return await userRepository.AddAsync(newUser);
    }

    public async Task<(User? user, bool resultado)> BootstrapAdminUser()
    {
        var userCount = await userRepository.Count();

        if (userCount > 0)
        {
            return (null, false);
        }

        var adminUser = new User()
        {
            Id = Guid.NewGuid(),
            UserName = configuration["Database:DefaultUserName"]!,
            Professor = null,
            Role = "admin"

        };

        adminUser.PassWordHash = hasher.HashPassword(adminUser, configuration["Database:DefaultPassword"]!);

        var createdUser = await userRepository.AddAsync(adminUser);
        
        if (createdUser is null)
        {
            return (null, false);
        }

        return (createdUser, true);

    }
    
    public  async Task<IResult> Login(LoginDto userInput)
    {
        var user = await userRepository.FindByUserNameAsync(userInput.UserName);
        
        if (user == null) return Results.Unauthorized();
        
        


        var result = hasher.VerifyHashedPassword(user, user.PassWordHash, userInput.PassWord);

        if (result == PasswordVerificationResult.Failed)
            return Results.Unauthorized();

        var token = GenerateToken(user.UserName, user.Role);
        return Results.Ok(new { token });
    }
}