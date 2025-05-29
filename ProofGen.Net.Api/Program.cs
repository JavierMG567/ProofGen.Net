using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProofGen.Net.Api.Endpoints;
using ProofGen.Net.Application.Services;
using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IImageProcessor, ImageProcessor>();
builder.Services.AddScoped<IOcrEngine, TesseractOcrEngine>();
builder.Services.AddScoped<ITicketParser, TicketParser>();
builder.Services.AddScoped<ITicketExtractor, TicketExtractorService>();
builder.Services.AddScoped<IProductExtractor , ProductExtractor>();
builder.Services.AddScoped<IMetadataRetrieveTicket,  MetadataRetrieveTicket>();
builder.Services.AddScoped<ITicketPdfService,  TicketPdfService>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ticket OCR API", Version = "v1" });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();  // Sirve index.html por defecto
app.UseStaticFiles();   // Habilita wwwroot

app.UseCors(); // <<--- MUY IMPORTANTE: antes de cualquier endpoint

// Endpoints
app.MapExtraction();
app.MapGeneration();

app.Run();
