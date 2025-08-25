using Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Database;

public class AgendamentosDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;
    public DbSet<Ambiente> Ambientes { get; set; }
    public DbSet<Horario> Horarios { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Professor> Professores { get; set; }

    public DbSet<Session> Sessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(configuration["Database:ConnString"]);

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
        
        modelBuilder.Entity<Session>().HasIndex(s =>  s.RefreshToken)
            .HasDatabaseName("IX_Session_RefreshToken")
            .IsUnique();
        
        modelBuilder.Entity<Session>().HasIndex(s => s.ExpiresAt)
            .HasDatabaseName("IX_Session_ExpiresAt");

        
    }
}