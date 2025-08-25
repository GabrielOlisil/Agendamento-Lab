namespace Models.DTOs.Agendamento;

public class AgendamentoPatchDto
{
    public Guid? AmbienteId { get; set; }
    public DateTime? Data { get; set; }
    public Guid? HorarioId { get; set; }
    public Guid? ProfessorId { get; set; }
}