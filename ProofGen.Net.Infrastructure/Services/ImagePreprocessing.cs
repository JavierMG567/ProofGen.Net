using OpenCvSharp;
using ProofGen.Net.Domain.Interfaces;

namespace ProofGen.Net.Infrastructure.Services;

public class ImagePreprocessing : IImagePreprocessing
{
    public async Task<byte[]> Preprocess(string imagePath)
    {
        try
        {
            using var src = Cv2.ImRead(imagePath, ImreadModes.Grayscale);
            var processed = new Mat();
    
            Cv2.GaussianBlur(src, processed, new Size(3, 3), 0);
            Cv2.AdaptiveThreshold(processed, processed, 255,
            AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 11, 2);
            Cv2.ImEncode(".png", processed, out var imageData);
        
            return await Task.FromResult(imageData.ToArray());
        }
        catch (Exception ex)
        {
            throw new Exception($"Error relacionado con OpenCvSharp: {ex.Message}", ex);
        }
    }
}
