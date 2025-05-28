using System.ComponentModel.DataAnnotations;

namespace Agendamentos.Domain.DTOs;

public record AmbienteCreateDto
{
    [Required]
    public required string Nome { get; init; }

}