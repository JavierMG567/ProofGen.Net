using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Infrastructure.Services;

public class AzureOcrService : IAzureOcrService
{
    public async Task<string> TryAzureOCR(byte[] imageBytes)
    {
        string endpoint = "https://<TU-ENDPOINT>.cognitiveservices.azure.com/";
        string subscriptionKey = "<TU-KEY>";

        var client = new ComputerVisionClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { })
        {
            Endpoint = endpoint
        };

        using var imageStream = new MemoryStream(imageBytes);
        var textHeaders = await client.ReadInStreamAsync(imageStream);
        string operationLocation = textHeaders.OperationLocation;
        string operationId = operationLocation.Split('/').Last();

        ReadOperationResult result;
        do
        {
            await Task.Delay(1000);
            result = await client.GetReadResultAsync(Guid.Parse(operationId));
        } while (result.Status == OperationStatusCodes.Running);

        if (result.Status == OperationStatusCodes.Succeeded)
        {
            var lines = result.AnalyzeResult.ReadResults
                .SelectMany(r => r.Lines)
                .Select(l => l.Text);
            return string.Join("\n", lines);
        }

        return "";
    }
}
