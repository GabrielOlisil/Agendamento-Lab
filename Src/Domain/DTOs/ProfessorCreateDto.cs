using System.ComponentModel.DataAnnotations;

namespace Agendamentos.Domain.DTOs;

public class ProfessorCreateDto
{
    [Required]
    public string Nome { get; set; }
    [Required]
    public  string Matricula { get; set; }

}