using Agendamentos.Domain.DTOs.Agendamento;

namespace Agendamentos.Domain.DTOs.Calendario;

public record CalendarioDiaResponseDto()
{
    public List<AgendamentoLabelResponseDto> Matutino { get; set; }
    public List<AgendamentoLabelResponseDto> Vespertino { get; set; }
    public List<AgendamentoLabelResponseDto> Noturno { get; set; }
}