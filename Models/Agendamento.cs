namespace Models;

public class Agendamento : AppModelBase
{
    public Ambiente Ambiente { get; set; }

    public DateTime Data { get; set; }

    public Horario Horario { get; set; }

    public Professor Professor { get; set; }
}