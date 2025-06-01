using NHunspell;
using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProofGen.Net.Infrastructure.Services;

public class OcrTextAuditor : IOcrTextAuditor
{
    public bool IsTextCoherent(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        if (text.Length < 20) return false;

        if (Regex.IsMatch(text, @"[^a-zA-Z0-9\s\.,;:¡!¿?\-()]")) return false;

        int totalWords = 0;
        int misspelledWords = 0;

        using (Hunspell hunspell = new Hunspell("es_ES.aff", "es_ES.dic")) // diccionario español
        {
            var words = Regex.Matches(text, @"\b\w+\b").Select(m => m.Value);
            foreach (var word in words)
            {
                totalWords++;
                if (!hunspell.Spell(word))
                {
                    misspelledWords++;
                }
            }
        }

        double errorRate = (double)misspelledWords / totalWords;
        return errorRate < 0.3; // umbral de tolerancia
    }
}
