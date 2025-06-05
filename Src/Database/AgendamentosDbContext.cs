using Agendamentos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Database;

public class AgendamentosDbContext : DbContext
{
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;
    public DbSet<Ambiente> Ambientes { get; set; }
    public DbSet<Horario> Horarios { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Professor> Professores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=agendamento;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        modelBuilder.Entity<Agendamento>(e => e.Property(p => p.Data)
            .HasColumnType("date"));


        modelBuilder.Entity<Ambiente>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(p => p.UserName)
            .IsUnique();

        modelBuilder.Entity<Horario>().HasData( // para fins de desenvolvimento
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Inicio = new TimeSpan(8, 0, 0),
                Rank = 1,
                Turno = Turno.Matutino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Inicio = new TimeSpan(9, 0, 0),
                Rank = 2,
                Turno = Turno.Matutino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Inicio = new TimeSpan(10, 0, 0),
                Rank = 3,
                Turno = Turno.Matutino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 4,
                Turno = Turno.Matutino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 5,
                Turno = Turno.Verspertino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 6,
                Turno = Turno.Verspertino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 7,
                Turno = Turno.Verspertino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 8,
                Turno = Turno.Verspertino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 9,
                Turno = Turno.Verspertino,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("10101010-1010-1010-1010-101010101010"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 10,
                Turno = Turno.Noturno,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("11011011-1101-1101-1101-110110110110"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 11,
                Turno = Turno.Noturno,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("12121212-1212-1212-1212-121212121212"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 12,
                Turno = Turno.Noturno,
                Deleted = false
            },
            new
            {
                Id = Guid.Parse("13131313-1313-1313-1313-131313131313"),
                Inicio = new TimeSpan(11, 0, 0),
                Rank = 13,
                Turno = Turno.Noturno,
                Deleted = false
            }
        );
    }
}