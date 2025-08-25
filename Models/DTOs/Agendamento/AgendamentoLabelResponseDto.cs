namespace Models.DTOs.Agendamento;

public record AgendamentoLabelResponseDto()
{
    public int Rank { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool Status { get; set; } = false;
}