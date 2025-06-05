using Agendamentos.Services;

namespace Agendamentos.Domain.Models;

public class Ambiente: AppModelBase
{
    public string Nome { get; set; }

    public string Slug { get; private set; }

    public void SetSlug(string value)
    {
        Slug = SlugService.Generate(value);
    }
}