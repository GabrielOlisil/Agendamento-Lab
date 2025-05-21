using Agendamentos.Database;
using Agendamentos.Endpoints;
using Agendamentos.Domain.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AgendamentosDbContext>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

var agendamentos = app.MapGroup("/agendamentos");

agendamentos.MapGet("/", AgendamentoEndpoints.ListAgendamento)
    .WithName("Agendar Aula")
    .WithOpenApi();

agendamentos.MapPost("/", AgendamentoEndpoints.CreateAgendamento);



var horarios = app.MapGroup("/horarios");

horarios.MapGet("/", HorarioEndpoints.ListarHorarios).WithName("Listar Horarios")
    .WithSummary("Lista todos os horarios")
    .WithOpenApi();


var ambientes = app.MapGroup("/ambientes");

ambientes.MapGet("/", AmbientesEndpoints.ListarAmbientes);
ambientes.MapPost("/", AmbientesEndpoints.CriarAmbiente);


var calendario = app.MapGroup("/calendario");

calendario.MapGet("/dia/{dia}", CalendarioEndpoints.AgendamentosDia);


app.Run();


