namespace Agendamentos.Domain.Models;

public class Horario
{
    public Guid Id { get; set; }
    public TimeSpan Inicio { get; set; }
    public int Rank { get; set; }
    public Turno Turno { get; set; }
}