using Microsoft.EntityFrameworkCore;
using ProofGen.Net.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Infrastructure.Persistence;

public class ProofGenDbContext : DbContext
{
    public DbSet<Ticket> Tickets {  get; set; }
    public ProofGenDbContext(DbContextOptions<ProofGenDbContext> options) : base(options) { }  
}
