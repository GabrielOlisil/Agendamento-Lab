using System.Text;
using Agendamentos.Database;
using Models;
using Agendamentos.Helpers;
using Agendamentos.Repositories;
using Agendamentos.Repositories.Interfaces;
using Agendamentos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        services.AddScoped<SessionService>();
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
                jwtOptions.Authority = Environment.GetEnvironmentVariable("HOST_DEFAULT_URL")!;
                jwtOptions.Audience = Environment.GetEnvironmentVariable("HOST_DEFAULT_URL")!;
                jwtOptions.RequireHttpsMetadata = false;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = Environment.GetEnvironmentVariable("HOST_DEFAULT_URL")!,
                    ValidAudience = Environment.GetEnvironmentVariable("HOST_DEFAULT_URL")!,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Secrets:JwtKey"]!)
                    )
                };
            });

        return services;
    }
    
    public static IServiceProvider ApplyMigrations(this IServiceProvider serviceProvider)
    {
        
        using var scope = serviceProvider.CreateScope();


        
        var context = scope.ServiceProvider.GetRequiredService<AgendamentosDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>(); 


        var pendingMigrations = context.Database.GetPendingMigrations();

        var migrations = pendingMigrations as string[] ?? pendingMigrations.ToArray();
        
        if (migrations.Length != 0)
        {
            logger.LogInformation("Applying {Count} pending migrations...", migrations.Length);
            context.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
        }
        else
        {
            logger.LogInformation("No pending migrations found.");
        }
        

        return serviceProvider;
    }
}