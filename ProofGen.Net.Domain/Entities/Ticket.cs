using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Domain.Entities;
public record Ticket(
    string LegalName,
    string FederalTaxpayerRegistry,
    DateTime Date,
    string Hours,
    string IdFolder,
    string Cashier,
    string CheckOut,
    string Address,
    decimal TotalAmount,
    decimal Card,
    decimal Change,
    List<Product> Products
);