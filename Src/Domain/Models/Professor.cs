using Agendamentos.Services;

namespace Agendamentos.Domain.Models;

public class Professor
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Slug { get; private set; }

    public string Matricula { get; set; }

    public void SetSlug(string value)
    {
        Slug = SlugService.Generate(value);
    }
}