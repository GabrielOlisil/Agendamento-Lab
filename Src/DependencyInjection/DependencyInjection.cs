using System.Text;
using Agendamentos.Domain.Models;
using Agendamentos.Helpers;
using Agendamentos.Repositories;
using Agendamentos.Repositories.Interfaces;
using Agendamentos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace Agendamentos.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<AgendamentoHelper>();
        services.AddScoped<IApplicationRepository<Professor>, ProfessorRepository>();
        services.AddScoped<IUserRepository<User>, UserRepository>();
        services.AddScoped<IApplicationRepository<Ambiente>, AmbienteRepository>();
        services.AddScoped<IApplicationRepository<Agendamento>, AgendamentoRepository>();
        services.AddScoped<IApplicationRepository<Horario>, HorarioRepository>();
        services.AddScoped<ProfessorService>();
        services.AddScoped<UserService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Insira o token JWT no formato: **&lt;seu_token&gt;**"

            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = "http://localhost:8080";
                jwtOptions.Audience = "http://localhost:8080";
                jwtOptions.RequireHttpsMetadata = false;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "http://localhost:8080",
                    ValidAudience = "http://localhost:8080",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Secrets:JwtKey"]!)
                    )
                };
            });

        return services;
    }
}