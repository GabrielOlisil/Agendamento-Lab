using Agendamentos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Database;

public class AgendamentosDbContext : DbContext
{
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;
    public DbSet<Ambiente> Ambientes { get; set; }
    public DbSet<Horario> Horarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=agendamento;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agendamento>(e => e.Property(p => p.Data)
            .HasColumnType("date"));
        
        modelBuilder.Entity<Horario>().HasData(
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Inicio = new TimeSpan(8, 0, 0),
                Rank = 1, 
                Turno = Turno.Matutino
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Inicio = new TimeSpan(9, 0, 0),
                Rank = 2, 
                Turno = Turno.Matutino
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Inicio = new TimeSpan(10, 0, 0),
                Rank = 3, 
                Turno = Turno.Matutino
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 4, 
                Turno = Turno.Matutino
            });
    }
}