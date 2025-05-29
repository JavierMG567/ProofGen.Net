using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Domain.Entities;
public record Product(
    string Description,
    decimal Price,
    decimal Discount,
    decimal Ammount,
    int Quantity
);