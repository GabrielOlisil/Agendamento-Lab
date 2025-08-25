namespace Models.DTOs.Agendamento;

public class AgendamentoLoteCreateDto
{
    public Guid AmbienteId { get; set; }
    public Guid ProfessorId { get; set; }
    
    // A data do primeiro dia a ser agendado (ex: a primeira segunda-feira)
    public DateTime DataInicio { get; set; }
    
    public Guid HorarioInicioId { get; set; }
    public Guid HorarioFimId { get; set; }
    
    // Quantas semanas o agendamento deve se repetir
    public int SemanasAReplicar { get; set; }
}