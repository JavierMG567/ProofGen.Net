using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Domain.Interfaces;
public interface IImageProcessor
{
    Task<string> ProcessAndSaveTemporary(IFormFile image);
}
