namespace Agendamentos.Domain.Models;

public abstract class AppModelBase
{
    public Guid Id { get; init; }
    public bool Deleted { get; set; } = false;
}