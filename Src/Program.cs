using Agendamentos.Database;
using Agendamentos.Endpoints;
using Agendamentos.Domain.Models;
using Agendamentos.Helpers;
using Agendamentos.Repositories;
using Agendamentos.Repositories.Interfaces;
using Agendamentos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<AgendamentosDbContext>();
builder.Services.AddScoped<AgendamentoHelper>();
builder.Services.AddScoped<IApplicationRepository<Professor>, ProfessorRepository>();
builder.Services.AddScoped<IApplicationRepository<User>, UserRepository>();
builder.Services.AddScoped<ProfessorService>();
builder.Services.AddScoped<UserService>();


builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.Authority = "https://localhost:8080";
        jwtOptions.Audience = "https://localhost:8080";
    });

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

calendario.MapGet("/{slug}/dia/{dia:datetime}", CalendarioEndpoints.AgendamentosDia);
calendario.MapGet("/{slug}/semana/{dia:datetime}", CalendarioEndpoints.AgendamentosSemana);


var professores = app.MapGroup("professores");

professores.MapPost("/", ProfessoresEndpoints.CreateProfessor);
professores.MapGet("/", ProfessoresEndpoints.ListarProfessores);

var users = app.MapGroup("users");

users.MapPost("/bootstrap", UserEndpoints.BootstrapUser);

app.Run();


