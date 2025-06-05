using Agendamentos.Database;
using Agendamentos.Domain.DTOs.Calendario;
using Agendamentos.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Agendamentos.Controllers;

[ApiController]
[Route("[controller]")]
public class CalendarioController(AgendamentoHelper agendamentoHelper) : ControllerBase
{
    [HttpGet("{slug}/dia/{dia:datetime}")]
    public async Task<IActionResult> AgendamentosDia(string slug, DateTime dia)
    {

        var aulas = await agendamentoHelper.ObterAgendamentosDia(slug, dia);


        return Ok(aulas);
    }



    [HttpGet("{slug}/semana/{dia:datetime}")]
    public async Task<IActionResult> AgendamentosSemana(string slug, DateTime dia)
    {

        var diferenca = dia.DayOfWeek - DayOfWeek.Sunday;
        DateTime domingo = dia.AddDays(-diferenca);

        CalendarioDiaResponseDto[] aulas = new CalendarioDiaResponseDto[7];

        for (int i = 0; i < 7; i++)
        {
            var data = domingo.AddDays(i);
            aulas[i] = await agendamentoHelper.ObterAgendamentosDia(slug, data);
        }


        return Ok(aulas);
    }
}