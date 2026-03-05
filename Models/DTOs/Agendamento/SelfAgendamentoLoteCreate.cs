namespace Models.DTOs.Agendamento;

public class SelfAgendamentoLoteCreateDto
{
    public Guid AmbienteId { get; set; }
    public DateTime DataInicio { get; set; }
    public Guid HorarioInicioId { get; set; }
    public Guid HorarioFimId { get; set; }
    public int SemanasAReplicar { get; set; }
}