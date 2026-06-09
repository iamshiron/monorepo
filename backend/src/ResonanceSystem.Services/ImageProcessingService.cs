using SkiaSharp;

namespace Shiron.ResonanceSystem.Services;

public interface IImageProcessingService {
    SKBitmap? BinarizeFromBytes(byte[] buffer, byte threshold = 128);
}

public class ImageProcessingService : IImageProcessingService {
    public SKBitmap? BinarizeFromBytes(byte[] buffer, byte threshold = 128) {
        using var originalBitmap = SKBitmap.Decode(buffer);

        if (originalBitmap == null) {
            return null;
        }

        var binarizedBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
        using var canvas = new SKCanvas(binarizedBitmap);

        using var grayscaleFilter = SKColorFilter.CreateColorMatrix(new float[] {
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0, 0, 0, 1, 0
        });

        var lut = new byte[256];
        var alphaIdentity = new byte[256];
        for (var i = 0; i < 256; i++) {
            lut[i] = (byte) (i >= threshold ? 255 : 0);
            alphaIdentity[i] = (byte) i;
        }

        using var thresholdFilter = SKColorFilter.CreateTable(alphaIdentity, lut, lut, lut);
        using var binarizeFilter = SKColorFilter.CreateCompose(thresholdFilter, grayscaleFilter);

        using var paint = new SKPaint {
            ColorFilter = binarizeFilter,
            IsAntialias = false // Keeps edges sharp for OCR
        };

        canvas.DrawBitmap(originalBitmap, 0, 0, paint);
        return binarizedBitmap;
    }
}
