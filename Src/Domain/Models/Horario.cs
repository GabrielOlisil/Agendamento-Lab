namespace Agendamentos.Domain.Models;

public class Horario : AppModelBase
{
    public TimeSpan Inicio { get; set; }
    public int Rank { get; set; }
    public Turno Turno { get; set; }
}