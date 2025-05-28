using Agendamentos.Database;
using Agendamentos.Domain.DTOs;
using Agendamentos.Domain.Models;
using Agendamentos.Helpers;
using Agendamentos.Repositories;
using Agendamentos.Repositories.Interfaces;
using Agendamentos.Services;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Endpoints;

public class ProfessoresEndpoints
{
    public static async Task<IResult> CreateProfessor(
        AgendamentoHelper agHelper,
        ProfessorCreateDto professorDto,
        IApplicationRepository<Professor> professorRepository,
        ProfessorService professorService
    )
    {
        var professor = new Professor(
            Guid.NewGuid(),
            professorDto.Nome,
            professorDto.Matricula,
            professorDto.Nome);

        var professorCriado = await professorService.CreateNewProfessorAndHisUserAsync(professor);


        if (professorCriado is null)
        {
            return TypedResults.BadRequest("Não foi possível criar o professor.");
        }


        return TypedResults.Created($"/professores/{professorCriado.Id}", professorCriado);
    }

    public static async Task<IResult> ListarProfessores(
        IApplicationRepository<Professor> professorRepository
    )
    {
        var professores = await professorRepository.GetAll();

        return TypedResults.Ok(professores);
    }
}