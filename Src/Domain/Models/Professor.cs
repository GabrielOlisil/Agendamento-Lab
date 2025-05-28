using Agendamentos.Services;

namespace Agendamentos.Domain.Models;

public class Professor
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Slug { get;  set; }

    public string Matricula { get; set; }
    
    public Professor()
    {
        
    }

    public Professor(Guid id, string nome, string matricula,string slug)
    {
        Id = id;
        Nome = nome;
        Matricula = matricula;
        Slug = SlugService.Generate(slug);
    }
}