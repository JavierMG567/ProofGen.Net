using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProofGen.Net.Application.Services;
public class ProductExtractor : IProductExtractor
{
    public List<Product> Extract(string text)
    {
        var lines = GetNonEmptyLines(text);
        var products = new List<Product>();
        Product currentProduct = null!;

        foreach (var line in lines)
        {
            var match = MatchProductLine(line);
            if (match.Success)
            {
                if (currentProduct != null) products.Add(currentProduct);
                currentProduct = ParseProductFromMatch(match);
            }
            else if (currentProduct != null)
            {
                if (IsContinuationLine(line)) currentProduct = currentProduct with { Description = currentProduct.Description + " " + line };
                else
                {
                    products.Add(currentProduct);
                    currentProduct = null!;
                }
            }
        }

        if (currentProduct != null) products.Add(currentProduct);
        return products;
    }

    private string[] GetNonEmptyLines(string text)
    {
        List<string> result = new List<string>();
        string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                result.Add(trimmed);
            }
        }

        return result.ToArray();
    }

    private bool IsContinuationLine(string line)
    {
        return !Regex.IsMatch(line, @"\d+\.\d{2}") &&
               !Regex.IsMatch(line, @"Total|IVA|Tarjeta|Cambio|Cant\.|Descripción", RegexOptions.IgnoreCase);
    }

    private Match MatchProductLine(string line)
    {
        string pattern = @"^\s*([0-9A-Za-z\u00FC\u00C4\u00D6\u00DC\u00DF])\s+(.+?)\s+(\d+\.\d{2})\s+(\d+\.\d{2})\s+(\d+\.\d{2})\s*$";
        return Regex.Match(line, pattern, RegexOptions.IgnoreCase);
    }

    private Product ParseProductFromMatch(Match match)
    {
        int quantity = ParseQuantity(match.Groups[1].Value);

        return new Product(
            Description: match.Groups[2].Value.Trim(),
            Price: SetDecimal(match.Groups[3].Value),
            Discount: SetDecimal(match.Groups[4].Value),
            Ammount: SetDecimal(match.Groups[5].Value),
            Quantity: quantity
        );
    }

    private int ParseQuantity(string raw)
    {
        return int.TryParse(raw, out int q) ? q :
               raw.Equals("ü", StringComparison.OrdinalIgnoreCase) ? 2 :
               raw.Equals("A", StringComparison.OrdinalIgnoreCase) ? 3 : 1;
    }

    private decimal SetDecimal(string raw)
    {
        return decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
            ? value : 0;
    }
}
