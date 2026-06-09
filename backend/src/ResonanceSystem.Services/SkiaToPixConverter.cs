namespace Shiron.ResonanceSystem.Services;

using System;
using SkiaSharp;
using Tesseract;

public static class SkiaToPixConverter {
    /// <summary>
    /// Converts an SKBitmap directly to a Tesseract Pix using zero-allocation unmanaged memory copying.
    /// Caller is responsible for disposing the returned Pix.
    /// </summary>
    public static Pix ConvertBitmapToPix(SKBitmap bitmap) {
        var width = bitmap.Width;
        var height = bitmap.Height;
        var depth = bitmap.BytesPerPixel * 8;

        var pix = Pix.Create(width, height, depth);
        var pixData = pix.GetData();

        var skiaStride = bitmap.RowBytes;
        var leptonicaStride = pixData.WordsPerLine * 4;

        unsafe {
            var srcBase = (byte*) bitmap.GetPixels().ToPointer();
            var dstBase = (byte*) pixData.Data.ToPointer();

            var copyLength = Math.Min(skiaStride, leptonicaStride);

            for (var y = 0; y < height; y++) {
                var srcRow = srcBase + y * skiaStride;
                var dstRow = dstBase + y * leptonicaStride;
                Buffer.MemoryCopy(srcRow, dstRow, leptonicaStride, copyLength);
            }
        }

        return pix;
    }
}
