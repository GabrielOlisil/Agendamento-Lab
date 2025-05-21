namespace Agendamentos.Domain.Models;


public class Agendamento
{
    public Guid Id { get; init; }

    public Ambiente Ambiente { get; set; } = null!;

    public DateTime Data { get; set; }

    public Horario Horario { get; set; } = null!;
}