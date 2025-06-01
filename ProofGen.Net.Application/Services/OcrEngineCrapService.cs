using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Application.Services;

public class OcrEngineCrapService : IOcrEngineCrapService
{
    private readonly IImagePreprocessing _preprocessor;
    private readonly IOcrEngine _tesseractOcr;
    private readonly IAzureOcrService _azureOcrService;

    public OcrEngineCrapService(
        IImagePreprocessing preprocessor,
        IOcrEngine tesseractOcr,
        IAzureOcrService azureOcrService)
    {
        _preprocessor = preprocessor;
        _tesseractOcr = tesseractOcr;
        _azureOcrService = azureOcrService;
    }

    public async Task<string> ExtractText(string imagePath)
    {
        byte[] imageBytes;

        try
        {
            imageBytes = await _preprocessor.Preprocess(imagePath);
        }
        catch
        {
            imageBytes = await File.ReadAllBytesAsync(imagePath); // Fallback si OpenCV falla
        }

        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
        try
        {
            await File.WriteAllBytesAsync(tempPath, imageBytes);

            var textTesseract = await TryTesseract(tempPath);

            if (!string.IsNullOrWhiteSpace(textTesseract) && textTesseract.Length > 40)
                return textTesseract;

            var textAzure = await TryAzure(imageBytes);

            return string.IsNullOrWhiteSpace(textAzure) ? textTesseract : textAzure;
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private async Task<string> TryTesseract(string imagePath)
    {
        try
        {
            return await _tesseractOcr.ExtractText(imagePath);
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<string> TryAzure(byte[] imageBytes)
    {
        try
        {
            return await _azureOcrService.TryAzureOCR(imageBytes);
        }
        catch
        {
            return string.Empty;
        }
    }
}
