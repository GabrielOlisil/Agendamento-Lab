
namespace Models.DTOs.User;

public record UserCreateDto
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
}