using Agendamentos.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
public class HorarioController(AgendamentosDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListarHorarios()
    {
        return Ok(await dbContext.Horarios.OrderBy(p => p.Rank).ToListAsync());
    }
}