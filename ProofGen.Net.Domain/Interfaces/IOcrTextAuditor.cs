using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Domain.Interfaces;

public interface IOcrTextAuditor
{
    bool IsTextCoherent(string text);
}
