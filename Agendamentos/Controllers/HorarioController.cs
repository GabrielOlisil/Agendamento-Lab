using Agendamentos.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs.Horario;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
public class HorarioController(AgendamentosDbContext dbContext, ILogger<HorarioController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListarHorarios()
    {
        return Ok(await dbContext.Horarios.OrderBy(p => p.Rank).ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "admin")]    public async Task<IActionResult> CreateHorario(HorarioCreateDto horario)
    {

        if (await dbContext.Horarios.AnyAsync(h => h.Rank == horario.Rank))
        {
            return BadRequest("Já existe um Horario com este Rank");
        }
        
        Horario newHorario = new()
            { Inicio = horario.Inicio, Rank = horario.Rank, Turno = horario.Turno, Id = Guid.NewGuid() };

        try
        {
            dbContext.Horarios.Add(newHorario);
            await dbContext.SaveChangesAsync();
            return  Ok(newHorario);
        }
        catch (Exception e)
        {
            logger.LogError("Erro ao criar Horario: {0} \nat: {1}", e.Message, e.StackTrace);
            
            return BadRequest("Não foi possível criar o horario");
        }
    }
}