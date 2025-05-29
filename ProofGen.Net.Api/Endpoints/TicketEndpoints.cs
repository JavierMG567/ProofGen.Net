using Microsoft.AspNetCore.Mvc;
using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;

namespace ProofGen.Net.Api.Endpoints;
public static class TicketEndpoints
{
    public static void MapExtraction(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/tickets/extraer", async (HttpRequest request, ITicketExtractor extractor) =>
        {
            var form = await request.ReadFormAsync();
            var archivo = form.Files.FirstOrDefault();

            if (archivo == null || archivo.Length == 0)
                return Results.BadRequest("Archivo no proporcionado");

            var resultado = await extractor.Extract(archivo);
            return Results.Ok(resultado);
        })
       .Accepts<IFormFile>("multipart/form-data")
       .Produces<Ticket>(StatusCodes.Status200OK)
       .WithName("ExtraerTicket")
       .WithTags("Tickets");
    }

    public static void MapGeneration(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/ticket/pdf", ([FromBody] Ticket ticket, ITicketPdfService ticketPdfService) =>
        {
            if (ticket == null)
            {
                return Results.BadRequest("Datos del ticket inválidos");
            }

            var pdfBytes = ticketPdfService.Generate(ticket);

            return Results.File(pdfBytes, "application/pdf", "ticket.pdf");
        })
        .WithName("GenerarPdfTicket")
        .WithTags("PDF");
    }
}
