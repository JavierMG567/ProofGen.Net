using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProofGen.Net.Api.Endpoints;
using ProofGen.Net.Application.Handlers;
using ProofGen.Net.Application.Services;
using ProofGen.Net.Domain.Entities;
using ProofGen.Net.Domain.Interfaces;
using ProofGen.Net.Infrastructure.Persistence.DependencyInjection;
using ProofGen.Net.Infrastructure.Persistence.Repositories;
using ProofGen.Net.Infrastructure.Services;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

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
app.MapPostBillet();

app.Run();
