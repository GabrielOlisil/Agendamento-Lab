namespace Agendamentos.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; }

    public string PassWordHash { get; set; }

    public Professor? Professor { get; set; }
    public string Role { get; set; }
}