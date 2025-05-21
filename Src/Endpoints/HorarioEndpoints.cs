using Agendamentos.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Endpoints;

public class HorarioEndpoints
{
    public static async Task<IResult> ListarHorarios(AgendamentosDbContext dbContext)
    {
        return TypedResults.Ok( await dbContext.Horarios.OrderBy(p => p.Rank).ToListAsync());
    }
}