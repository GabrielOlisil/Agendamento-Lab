namespace Models.DTOs.Agendamento;

public record SelfAgendamentoCreateDto()
{
    public Guid AmbienteId { get; set; }

    public DateTime Data { get; set; }

    public int HorarioRank { get; set; }
}