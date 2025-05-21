using Agendamentos.Database;
using Agendamentos.Domain.DTOs;
using Agendamentos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Endpoints;

public class AmbientesEndpoints
{
    public static async Task<IResult> ListarAmbientes(AgendamentosDbContext dbContext)
    {
        var ambientes = await dbContext.Ambientes.ToListAsync();
        return TypedResults.Ok(ambientes);
    }

    public static async Task<IResult> CriarAmbiente(AgendamentosDbContext dbContext, AmbienteCreateDto ambiente)
    {
        if (string.IsNullOrWhiteSpace(ambiente.Nome))
        {
            return TypedResults.BadRequest("O nome do ambiente é obrigatório.");
        }


        var ambienteNovo = new Ambiente()
        {
            Id = Guid.NewGuid(),
            Nome = ambiente.Nome,
        };

        dbContext.Ambientes.Add(ambienteNovo);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/ambientes/{ambienteNovo.Id}", ambienteNovo);
    }
}