using Microsoft.AspNetCore.Http;
using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;
using SixLabors.ImageSharp;                    
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text.Json;
using System.Net.Sockets;

namespace ProofGen.Net.Application.Services;
public class TicketExtractorService : ITicketExtractor
{
    private readonly IImageProcessor _imageProcessor;
    private readonly IOcrEngine _ocrEngine;
    private readonly ITicketParser _ticketParser;

    public TicketExtractorService(
        IImageProcessor imageProcessor, 
        IOcrEngine ocrEngine, 
        ITicketParser ticketParser
    )
    {
        _imageProcessor = imageProcessor;
        _ocrEngine = ocrEngine;
        _ticketParser = ticketParser;
    }

    public async Task<Ticket> Extract(IFormFile image, string fullName, string taxId, string invoiceTicketBillet)
    {
        string tempPath = null!;
        try
        {
            tempPath = await _imageProcessor.ProcessAndSaveTemporary(image);
            var ocrText = await _ocrEngine.ExtractText(tempPath);
            return _ticketParser.ParseTicket(ocrText, fullName, taxId, invoiceTicketBillet);
        }
        finally
        {
            if (!string.IsNullOrEmpty(tempPath) && File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
