namespace Models;

public class Lote : AppModelBase
{
    public required List<Agendamento> Grupo { get; set; }
}