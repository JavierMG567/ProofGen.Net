using Microsoft.AspNetCore.Mvc;
using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;

namespace ProofGen.Net.Api.Endpoints;

public static class TicketEndpoints
{
    public static void MapExtraction(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/tickets/extract", async (HttpRequest request, ITicketExtractor extractor) =>
        {
            var form = await request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();

            if (file == null || file.Length == 0)
                return Results.BadRequest("El archivo es obligatorio.");

            var invoiceTicketBillet = form["invoiceTicketBillet"].ToString();
            var fullName = form["fullName"].ToString();
            var taxId = form["taxId"].ToString();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(taxId) || string.IsNullOrWhiteSpace(invoiceTicketBillet))
                return Results.BadRequest("Todos los campos del formulario son requeridos.");

            try
            {
                var result = await extractor.Extract(file, fullName, taxId, invoiceTicketBillet);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al extraer el ticket: {ex.Message}");
            }

        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<Ticket>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("ExtractTicket")
        .WithTags("Tickets");
    }

    public static void MapGeneration(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/tickets/pdf", ([FromBody] Ticket ticket, ITicketPdfService ticketPdfService) =>
        {
            if (ticket == null)
                return Results.BadRequest("Datos del ticket inválidos.");

            try
            {
                var pdfBytes = ticketPdfService.Generate(ticket);
                return Results.File(pdfBytes, "application/pdf", "ticket.pdf");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error generando el PDF: {ex.Message}");
            }

        })
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("GenerateTicketPdf")
        .WithTags("PDF");
    }

    public static void MapPostBillet(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/billets", async ([FromBody] Ticket ticket, IBilletRepository billetRepository) =>
        {
            if (ticket == null)
                return Results.BadRequest("Datos del ticket inválidos.");

            try
            {
                await billetRepository.SaveAsync(ticket);
                return Results.Ok("Ticket guardado correctamente.");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al guardar el ticket: {ex.Message}");
            }

        })
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("SaveTicketBillet")
        .WithTags("Billets");
    }
}
