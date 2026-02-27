namespace Models.DTOs.Horario;

public record HorarioCreateDto(TimeSpan Inicio, int Rank, Turno Turno);