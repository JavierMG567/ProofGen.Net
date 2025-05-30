using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProofGen.Net.Application.Services;
public class TicketParser :ITicketParser
{
    private readonly IProductExtractor _productExtractor;
    private readonly IMetadataRetrieveTicket _metadataRetriever;
    public TicketParser(
        IProductExtractor productExtractor,
        IMetadataRetrieveTicket metadataRetriever)
    {
        _productExtractor = productExtractor;
        _metadataRetriever = metadataRetriever;
    }

    public Ticket ParseTicket(string text, string fullName, string taxId)
    {
        try
        {
            var products = _productExtractor.Extract(text);
            var ticket = _metadataRetriever.Retrieve(text, products, fullName, taxId);
            return ticket;
        }
        catch(Exception ex) 
        {
            throw new Exception(ex.Message);
        }
    }
}

