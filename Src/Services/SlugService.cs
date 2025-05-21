using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Agendamentos.Services;

public class SlugService
{
    public static string Generate(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
            return string.Empty;

        // 1. Remove acentos
        var str = RemoveAccents(phrase).ToLowerInvariant();

        // 2. Remove caracteres inválidos
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

        // 3. Normaliza espaços
        str = Regex.Replace(str, @"\s+", " ").Trim();

        // 4. Limita o tamanho
        str = str.Length <= 45 ? str : str.Substring(0, 45).Trim();

        // 5. Substitui espaços por hífens
        str = Regex.Replace(str, @"\s", "-");

        // 6. Remove hífens duplicados ou no fim/início
        str = Regex.Replace(str, "-{2,}", "-").Trim('-');

        return str;
    }

    private static string RemoveAccents(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}