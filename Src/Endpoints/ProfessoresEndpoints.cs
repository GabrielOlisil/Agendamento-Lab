using Agendamentos.Database;
using Agendamentos.Domain.DTOs;
using Agendamentos.Domain.Models;
using Agendamentos.Helpers;
using Agendamentos.Services;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Endpoints;

public class ProfessoresEndpoints
{
    public static async Task<IResult> CreateProfessor(
        AgendamentosDbContext dbContext, 
        AgendamentoHelper agHelper, 
        ProfessorCreateDto professor
        )
    {

        var slug = SlugService.Generate(professor.Nome);
        
        var professorNovo = new Professor()
        {
            Id = Guid.NewGuid(),
            Nome = professor.Nome,
            Matricula = professor.Matricula,
        };
        
        professorNovo.SetSlug(slug);

        dbContext.Professores.Add(professorNovo);

        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/professores/{professorNovo.Id}", professorNovo);

    }

    public static async Task<IResult> ListarProfessores(AgendamentosDbContext dbContext)
    {
        var professores = await dbContext.Professores.ToListAsync();
        
        return TypedResults.Ok(professores);
    }
}