using ProofGen.Net.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Domain.Interfaces;
public interface IMetadataRetrieveTicket
{
    Ticket Retrieve(string text, List<Product> products, string fullName, string taxId);
}
