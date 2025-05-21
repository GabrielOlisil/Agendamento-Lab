using Agendamentos.Domain.Models;

namespace Agendamentos.Domain.DTOs;

public record AgendamentoCreateDto
{
    public Guid AmbienteId { get; set; }

    public DateTime Data { get; set; }

    public Guid HorarioId { get; set; }
}