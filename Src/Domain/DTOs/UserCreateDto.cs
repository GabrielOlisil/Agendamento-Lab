using Agendamentos.Domain.Models;

namespace Agendamentos.Domain.DTOs;

public record UserCreateDto
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
}