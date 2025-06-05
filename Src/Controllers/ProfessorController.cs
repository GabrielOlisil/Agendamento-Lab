using Agendamentos.Domain.DTOs.ProfessorDto;
using Agendamentos.Domain.Models;
using Agendamentos.Helpers;
using Agendamentos.Repositories.Interfaces;
using Agendamentos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "admin")]
public class ProfessorController(
    IApplicationRepository<Professor> professorRepository,
    ProfessorService professorService
) : ControllerBase
{
    [HttpPost]
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

    [HttpGet]
    [Authorize(Roles = "admin, user")]
    public async Task<IActionResult> ListarProfessores()
    {
        var professores = await professorRepository.GetAll();

        return Ok(professores);
    }
}