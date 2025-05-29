using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Application.Services;
public class TicketPdfService :ITicketPdfService
{
    public byte[] Generate(Ticket ticket)
    {
        using var ms = new MemoryStream();
        using var writer = new PdfWriter(ms);
        using var pdf = new PdfDocument(writer);
        var doc = new iText.Layout.Document(pdf, new iText.Kernel.Geom.PageSize(226, 600)); // tamaño tipo ticket

        var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
        doc.SetFont(font).SetFontSize(9);

        doc.Add(new Paragraph("TICKET PARA FACTURACION").SetTextAlignment(TextAlignment.CENTER));
        doc.Add(new Paragraph(ticket.LegalName).SetTextAlignment(TextAlignment.CENTER));
        doc.Add(new Paragraph($"RFC: {ticket.FederalTaxpayerRegistry}"));
        doc.Add(new Paragraph($"Fecha: {ticket.Date}"));
        doc.Add(new Paragraph($"Hora: {ticket.Hours}"));
        doc.Add(new Paragraph($"Caja: {ticket.CheckOut}"));
        doc.Add(new Paragraph($"Cajero: {ticket.Cashier}"));

        doc.Add(new LineSeparator(new SolidLine()));

        foreach (var item in ticket.Products)
        {
            doc.Add(new Paragraph($"{item.Quantity} x {item.Description}"));
            doc.Add(new Paragraph($"${item.Ammount:F2}").SetTextAlignment(TextAlignment.RIGHT));
        }

        doc.Add(new LineSeparator(new SolidLine()));

        doc.Add(new Paragraph($"Total: ${ticket.TotalAmount:F2}").SetTextAlignment(TextAlignment.RIGHT));
        doc.Add(new Paragraph($"Tarjeta: ${ticket.Card:F2}").SetTextAlignment(TextAlignment.RIGHT));
        doc.Add(new Paragraph($"Cambio: ${ticket.Change:F2}").SetTextAlignment(TextAlignment.RIGHT));

        doc.Add(new LineSeparator(new SolidLine()));
        doc.Add(new Paragraph(ticket.Address).SetFontSize(8));

        doc.Close();
        return ms.ToArray();
    }
}
