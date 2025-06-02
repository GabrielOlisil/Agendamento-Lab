using Agendamentos.Domain.DTOs.ProfessorDto;
using Agendamentos.Domain.Models;
using Agendamentos.Helpers;
using Agendamentos.Repositories.Interfaces;
using Agendamentos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agendamentos.Controllers;

public class ProfessorController(
    IApplicationRepository<Professor> professorRepository,
    ProfessorService professorService
) : ControllerBase
{
    public async Task<IActionResult> CreateProfessor(
        ProfessorCreateDto professorDto
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
            return BadRequest("Não foi possível criar o professor.");
        }


        return Created($"/professores/{professorCriado.Id}", professorCriado);
    }

    public async Task<IActionResult> ListarProfessores()
    {
        var professores = await professorRepository.GetAll();

        return Ok(professores);
    }
}