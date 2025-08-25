using Models;

namespace Models.DTOs.User;

public record UserProfessorCreate()
{
    public string PassWord { get; set; }

    public Professor Professor { get; set; }

}