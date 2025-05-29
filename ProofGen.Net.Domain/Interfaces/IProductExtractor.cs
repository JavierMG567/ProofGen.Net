using ProofGen.Net.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Domain.Interfaces;
public interface IProductExtractor
{
    List<Product> Extract(string text);
}
