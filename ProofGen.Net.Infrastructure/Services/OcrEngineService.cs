using ProofGen.Net.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace ProofGen.Net.Infrastructure.Services;

public class OcrEngineService : IOcrEngine
{
    public async Task<string> ExtractText(string imagePath)
    {
        try
        {
            using var engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default);
            engine.SetVariable("tessedit_pageseg_mode", "11");

            using var pix = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(pix);
            string ocrText = page.GetText();

            return await Task.FromResult(ocrText);
        }
        catch (TesseractException ex)
        {
            throw new Exception($"Error relacionado con Tesseract: {ex.Message}", ex);
        }
    }
}
