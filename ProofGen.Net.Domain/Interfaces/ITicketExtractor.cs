using Microsoft.AspNetCore.Http;
using ProofGen.Net.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Domain.Interfaces;
public interface ITicketExtractor
{
    Task<Ticket> Extract(IFormFile image, string fullName, string taxId, string invoiceTicketBillet);
}
