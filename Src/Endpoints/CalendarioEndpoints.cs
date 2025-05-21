using System.Globalization;
using Agendamentos.Database;
using Agendamentos.Domain.Models;
using Agendamentos.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Endpoints;

public record CalendarioDiaResponse()
{
    public List<AgendamentoLabelResponse> Matutino { get; set; }
    public List<AgendamentoLabelResponse> Vespertino { get; set; }
    public List<AgendamentoLabelResponse> Noturno { get; set; }
}

public record AgendamentoLabelResponse()
{
    public int Rank { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool Status { get; set; } = false;
}

public class CalendarioEndpoints
{
    
    
    public static async Task<IResult> AgendamentosDia(string slug, DateTime dia, AgendamentosDbContext dbContext,
    AgendamentoHelper agendamentoHelper
    )
    {
        
        var aulas = await agendamentoHelper.ObterAgendamentosDia(slug, dia);


        return TypedResults.Ok(aulas);
    }




    public static async Task<IResult> AgendamentosSemana(string slug, DateTime dia, AgendamentoHelper agendamentoHelper)
    {

        var diferenca = dia.DayOfWeek - DayOfWeek.Sunday;
        DateTime domingo = dia.AddDays(-diferenca);
        
        CalendarioDiaResponse[] aulas = new CalendarioDiaResponse[7];
        
        for (int i = 0; i < 7; i++)
        {
            var data = domingo.AddDays(i);
            aulas[i] = await agendamentoHelper.ObterAgendamentosDia(slug, data);
        }
        
        
        return TypedResults.Ok(aulas);
    }
}