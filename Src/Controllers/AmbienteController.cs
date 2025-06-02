using Agendamentos.Database;
using Agendamentos.Domain.DTOs.Ambiente;
using Agendamentos.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
public class AmbienteController(AgendamentosDbContext dbContext): ControllerBase
{
    
    [HttpGet]
    public  async Task<IActionResult> ListarAmbientes()
    {
        var ambientes = await dbContext.Ambientes.ToListAsync();
        return Ok(ambientes);
    }

    [HttpPost]
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

        return Created($"/ambientes/{ambienteNovo.Id}", ambienteNovo);
    }
}