using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ProofGen.Net.Domain.Interfaces;
using ProofGen.Net.Infrastructure.Persistence.Repositories;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProofGen.Net.Application.Handlers;
using ProofGen.Net.Application.Services;
using ProofGen.Net.Infrastructure.Services;

namespace ProofGen.Net.Infrastructure.Persistence.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ProofGenDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IImageProcessor, ImageProcessor>();
        services.AddScoped<IOcrEngine, OcrEngineService>();
        services.AddScoped<ITicketParser, TicketParser>();
        services.AddScoped<ITicketExtractor, ExtractorServiceHanlder>();
        services.AddScoped<IProductExtractor, ProductExtractor>();
        services.AddScoped<IMetadataRetrieveTicket, MetadataRetrieveTicket>();
        services.AddScoped<ITicketPdfService, TicketPdfService>();
        services.AddScoped<IOcrEngineCrapService, OcrEngineCrapService>();
        services.AddScoped<IBilletRepository, BilletRepository>();
        services.AddScoped<IAzureOcrService, AzureOcrService>();
        services.AddScoped<IImagePreprocessing, ImagePreprocessing>();
        services.AddScoped<IOcrTextAuditor, OcrTextAuditor>();

        return services;
    }
}
