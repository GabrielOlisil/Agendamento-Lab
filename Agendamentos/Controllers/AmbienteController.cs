using Agendamentos.Database;
using Models.DTOs.Ambiente;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AmbienteController(AgendamentosDbContext dbContext) : ControllerBase
{

    [HttpGet]
    [AllowAnonymous] // Qualquer um pode listar os ambientes
    public async Task<IActionResult> ListarAmbientes()
    {
        var ambientes = await dbContext.Ambientes.Where(p => !p.Deleted).ToListAsync();
        return Ok(ambientes);
    }
    
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAmbiente(Guid id)
    {
        var ambiente = await dbContext.Ambientes.FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);
        if (ambiente == null)
        {
            return NotFound();
        }
        return Ok(ambiente);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CriarAmbiente(AmbienteCreateDto ambiente)
    {
        if (string.IsNullOrWhiteSpace(ambiente.Nome))
        {
            return BadRequest("O nome do ambiente é obrigatório.");
        }

        var ambienteNovo = new Ambiente()
        {
            Id = Guid.NewGuid(),
            Nome = ambiente.Nome,
        };
        ambienteNovo.SetSlug(ambienteNovo.Nome);

        dbContext.Ambientes.Add(ambienteNovo);
        await dbContext.SaveChangesAsync();

        return Created($"/ambiente/{ambienteNovo.Id}", ambienteNovo);
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateAmbiente(Guid id, AmbienteCreateDto ambienteUpdate)
    {
        var ambiente = await dbContext.Ambientes.FindAsync(id);
        if (ambiente == null || ambiente.Deleted)
        {
            return NotFound();
        }

        ambiente.Nome = ambienteUpdate.Nome;
        ambiente.SetSlug(ambienteUpdate.Nome);
        
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPatch("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PatchAmbiente(Guid id, [FromBody] AmbientePatchDto patchDto)
    {
        var ambiente = await dbContext.Ambientes.FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);
        if (ambiente == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(patchDto.Nome))
        {
            ambiente.Nome = patchDto.Nome;
            ambiente.SetSlug(patchDto.Nome);
        }
        
        await dbContext.SaveChangesAsync();
        return Ok(ambiente);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteAmbiente(Guid id)
    {
        var ambiente = await dbContext.Ambientes.FindAsync(id);
        if (ambiente == null)
        {
            return NotFound();
        }

        ambiente.Deleted = true; // Soft delete
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}