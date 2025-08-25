using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Ambiente;

public record AmbienteCreateDto
{
    [Required]
    public required string Nome { get; init; }

}