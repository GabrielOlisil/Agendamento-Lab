using Agendamentos.Domain.Models;

namespace Agendamentos.Domain.DTOs.User;

public record UserProfessorCreate()
{
    public string PassWord { get; set; }

    public Professor Professor { get; set; }

}