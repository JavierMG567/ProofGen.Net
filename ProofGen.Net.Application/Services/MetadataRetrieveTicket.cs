using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProofGen.Net.Application.Services;
public class MetadataRetrieveTicket : IMetadataRetrieveTicket
{
    private static readonly string[] SocialDraws = new[]
    {
        "S\\.A\\. de C\\.V\\.",
        "S\\. de R\\.L\\.",
        "S\\. de R\\.L\\. de C\\.V\\.",
        "A\\.C\\.",
        "S\\.C\\.",
        "S\\.A\\.",
        "S\\. en C\\.",
        "S\\. en N\\.C\\."
    };

    public Ticket Retrieve(string text, List<Product> products, string fullName, string taxId)
    {
        string legalName = ExtractCompanyName(text);
        string legalReference = ExtractRFC(text);
        string address = ExtractAddress(text, legalName);
        string idFolder = ExtractIdFolder(text);
        string cashier = ExtractOrGenerateId(text, "Cajero", 8);
        string checkOut = ExtractOrGenerateId(text, "Caja", 8);
        DateTime date = ExtractDate(text);
        string time = ExtractTime(text);
        decimal total = SetDecimal(text, @"Total\s*[:\$]*\s*(\d+(\.\d{1,2})?)");
        decimal card = SetDecimal(text, @"Tarjeta\s*[:\$]*\s*(\d+(\.\d{1,2})?)");
        decimal change = SetDecimal(text, @"Cambio\s*[:\$]*\s*(\d+(\.\d{1,2})?)");

        return new Ticket(
            FullName: fullName,
            TaxId: taxId,
            LegalName: legalName,
            FederalTaxpayerRegistry: legalReference,
            Date: date,
            Hours: time,
            IdFolder: idFolder,
            Cashier: cashier,
            CheckOut: checkOut,
            Address: address,
            TotalAmount: total,
            Card: card,
            Change: change,
            Products: products
        );
    }

    private string ExtractCompanyName(string text)
    {
        var match = Regex.Match(text, @"^(TIENDAS SUPER PRECIO\s*,\s*S\.A\.\s*de\s*C\.V\.)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : "";
    }

    private string ExtractRFC(string text)
    {
        var match = Regex.Match(text, @"RFC[:\s]*([A-Z0-9 ]{10,20})", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            string clean = match.Groups[1].Value.Replace(" ", "").Trim();
            if (Regex.IsMatch(clean, @"^[A-Z0-9]{12,13}$")) return clean;
        }
        return "";
    }

    private string ExtractAddress(string text, string companyName)
    {
        int startIdx = text.IndexOf(companyName, StringComparison.OrdinalIgnoreCase);
        int endIdx = text.IndexOf("EXPEDIDO EN", StringComparison.OrdinalIgnoreCase);
        if (startIdx != -1 && endIdx != -1 && endIdx > startIdx)
        {
            var block = text.Substring(startIdx + companyName.Length, endIdx - (startIdx + companyName.Length));
            var lines = block.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(l => l.Trim())
                             .Where(l => !IsNoiseLine(l))
                             .ToList();
            return string.Join(", ", lines);
        }

        var m1 = Regex.Match(text, @"(AV\s+.+?)\s+COL\.\s+(.+?)\s+C\.P\.\s+(\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (m1.Success)
        {
            return $"{m1.Groups[1].Value.Trim()}, COL. {m1.Groups[2].Value.Trim()}, C.P. {m1.Groups[3].Value.Trim()}";
        }

        var m2 = Regex.Match(text, @"(COL\..+?)\s+C\.P\.\s+(\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return m2.Success ? m2.Groups[0].Value.Trim() : "";
    }

    private bool IsNoiseLine(string line)
    {
        return Regex.IsMatch(line, @"\b(TORRE|PISO|OFICINA|RFC|C\.P\.|CD\. MEX\.|[A-Z\s]{5,})\b", RegexOptions.IgnoreCase);
    }

    private string ExtractIdFolder(string text)
    {
        var match = Regex.Match(text, @"(Folio|F-\s*alu)\s*(\d{10,})", RegexOptions.IgnoreCase);
        if (match.Success) return match.Groups[2].Value.Trim();

        var longNum = Regex.Match(text, @"\b\d{15,20}\b");
        if (longNum.Success) return longNum.Groups[0].Value.Trim();

        return Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
    }

    private string ExtractOrGenerateId(string text, string label, int randomDigits)
    {
        var match = Regex.Match(text, @$"{label}\s*(\d+)", RegexOptions.IgnoreCase);
        if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
            return match.Groups[1].Value.Trim();

        return GenerateRandomNumericString(randomDigits) + GenerateRandomNumericString(3);
    }

    private string GenerateRandomNumericString(int digits)
    {
        var rnd = new Random();
        return rnd.Next((int)Math.Pow(10, digits - 1), (int)Math.Pow(10, digits)).ToString();
    }

    private DateTime ExtractDate(string text)
    {
        string dateRaw = Regex.Match(text, @"Fecha\s*(\d{2}/\d{2}/\d{4})", RegexOptions.IgnoreCase).Groups[1].Value.Trim();
        return DateTime.TryParseExact(dateRaw, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt.Date : DateTime.Now.Date;
    }

    private string ExtractTime(string text)
    {
        string raw = Regex.Match(text, @"Hora\s*(\d{2}:\d{2}(?::\d{2})?\s*\d*)", RegexOptions.IgnoreCase).Groups[1].Value.Trim();
        var match = Regex.Match(raw, @"(\d{2}:\d{2})");
        return match.Success ? match.Groups[1].Value.Trim() : DateTime.Now.ToString("HH:mm");
    }

    private string ExtractLegalName(string text)
    {
        return text
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(linea => RegexEnterprisse.Match(linea.Trim()))
            .FirstOrDefault(match => match.Success)?
            .Groups["empresa"].Value.Trim()!;
    }

    private readonly Regex RegexEnterprisse = new Regex(
        @$"(?<empresa>.*?\b({string.Join("|", SocialDraws)})\b)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private decimal SetDecimal(string text, string pattern)
    {
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        return match.Success && decimal.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
            ? value : 0;
    }
}
