using Microsoft.AspNetCore.Http;
using ProofGen.Net.Domain.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofGen.Net.Application.Services;
public class ImageProcessor : IImageProcessor
{
    public async Task<string> ProcessAndSaveTemporary(IFormFile imagen)
    {
        if (imagen == null || !imagen.ContentType.StartsWith("image/"))
            throw new InvalidDataException("El archivo no es una imagen válida o está vacío.");

        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".jpg");

        using (var imageStream = imagen.OpenReadStream())
        using (var image = await Image.LoadAsync<Rgba32>(imageStream))
        {
            image.Mutate(x => x
                .Resize(image.Width * 2, image.Height * 2)
                .GaussianBlur(0.8f)
                .Contrast(3.13f)
                .GaussianSharpen()
                .Grayscale());

            await image.SaveAsync(tempPath);
        }

        return tempPath;
    }
}
