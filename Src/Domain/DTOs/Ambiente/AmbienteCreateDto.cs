using System.ComponentModel.DataAnnotations;

namespace Agendamentos.Domain.DTOs.Ambiente;

public record AmbienteCreateDto
{
    [Required]
    public required string Nome { get; init; }

}