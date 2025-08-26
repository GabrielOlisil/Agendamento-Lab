using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Models.DTOs.Auth;
using Models.DTOs.User;
using Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Services;

namespace Agendamentos.Services;

public class UserService(
    IUserRepository<User> userRepository,
    IPasswordHasher<User> hasher,
    IConfiguration configuration,
    SessionService sessionService
)
{
    public static string GenerateAccessToken(string username, string role, Guid id, DateTime expiresAt, IConfiguration configuration)
    {

        var expires = expiresAt;
        
        var key = Encoding.UTF8.GetBytes(configuration["Secrets:JwtKey"]!);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("HOST_DEFAULT_URL")!,
            audience: Environment.GetEnvironmentVariable("HOST_DEFAULT_URL")!,
            claims: claims,
            expires: expires,
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
            UserName = SlugService.Generate(userDto.Professor.Matricula)
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

    public async Task<(string token, string refreshToken)?> Login(LoginDto userInput)
    {
        var user = await userRepository.FindByUserNameAsync(userInput.UserName);

        if (user == null) return null;


        var result = hasher.VerifyHashedPassword(user, user.PassWordHash, userInput.PassWord);

        if (result == PasswordVerificationResult.Failed)
            return null;
        
        var displayName = user.Professor?.Nome ?? user.UserName;
        
        var refreshToken = await sessionService.SetSessionAsync(user);
        
        if (refreshToken == null) return null;
        
        
        var token = GenerateAccessToken(displayName, user.Role, user.Id, DateTime.UtcNow.AddHours(1), configuration);
        return (token, refreshToken);
    }
}