using Models.DTOs.ProfessorDto;
using Models;
using Agendamentos.Repositories.Interfaces;
using Agendamentos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Services;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProfessorController(
    IApplicationRepository<Professor> professorRepository,
    ProfessorService professorService
) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateProfessor(ProfessorCreateDto professorDto)
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

        return Created($"/professor/{professorCriado.Id}", professorCriado);
    }

    [HttpGet]
    [Authorize(Roles = "admin, user")]
    public async Task<IActionResult> ListarProfessores()
    {
        var professores = await professorRepository.GetAll();
        return Ok(professores);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin, user")]
    public async Task<IActionResult> GetProfessor(Guid id)
    {
        var professor = await professorRepository.GetById(id);
        if (professor == null || professor.Deleted)
        {
            return NotFound();
        }
        return Ok(professor);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateProfessor(Guid id, ProfessorCreateDto professorDto)
    {
        var professor = await professorRepository.GetById(id);
        if (professor == null || professor.Deleted)
        {
            return NotFound();
        }

        professor.Nome = professorDto.Nome;
        professor.Matricula = professorDto.Matricula;
        professor.Slug = SlugService.Generate(professorDto.Nome);

        await professorRepository.UpdateAsync(id, professor);
        return NoContent();
    }
    
    [HttpPatch("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PatchProfessor(Guid id, [FromBody] ProfessorPatchDto patchDto)
    {
        var professor = await professorRepository.GetById(id);
        if (professor == null || professor.Deleted)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(patchDto.Nome))
        {
            professor.Nome = patchDto.Nome;
            professor.Slug = SlugService.Generate(patchDto.Nome);
        }

        if (!string.IsNullOrEmpty(patchDto.Matricula))
        {
            professor.Matricula = patchDto.Matricula;
        }

        await professorRepository.UpdateAsync(id, professor);
        return Ok(professor);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteProfessor(Guid id)
    {
        var success = await professorRepository.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}