using Agendamentos.Domain.DTOs;
using Agendamentos.Domain.Models;
using Agendamentos.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Agendamentos.Services;

public class UserService(
    IApplicationRepository<User> userRepository,
    IPasswordHasher<User> hasher,
    IConfiguration configuration
    )
{

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
}