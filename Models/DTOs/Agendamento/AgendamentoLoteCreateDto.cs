namespace Models.DTOs.Agendamento;

public class AgendamentoLoteCreateDto
{
    public Guid AmbienteId { get; set; }
    public Guid ProfessorId { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public Guid HorarioInicioId { get; set; }
    public Guid HorarioFimId { get; set; }
    
    public int SemanasAReplicar { get; set; }
}