using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Infrastructure.Persistence.Repositories;

public class BilletRepository : IBilletRepository
{
    private readonly ProofGenDbContext _proofGenDbContext;

    public BilletRepository(ProofGenDbContext proofGenDbContext)
    {
        _proofGenDbContext = proofGenDbContext;
    }

    public async Task SaveAsync(Ticket ticket)
    {
        _proofGenDbContext.Tickets.Add(ticket);
        await _proofGenDbContext.SaveChangesAsync();
    }
}
