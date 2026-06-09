using Microsoft.Extensions.ObjectPool;
using Shiron.ResonanceSystem.Core.DTOs;
using SkiaSharp;
using Tesseract;

namespace Shiron.ResonanceSystem.Services;

public class TesseractEnginePolicy(string language) : IPooledObjectPolicy<TesseractEngine> {
    private readonly string _language = language;
    private readonly string _tessdataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");

    public TesseractEngine Create() {
        return new TesseractEngine(_tessdataPath, _language, EngineMode.Default);
    }
    public bool Return(TesseractEngine obj) {
        return true;
    }
}

public interface IOCRService {
    OCRResultDTO? Process(in SKBitmap data, in Rect? area = null, in PageSegMode? pageSegMode = null);
}

public class OCRService(ObjectPool<TesseractEngine> enginePool) : IOCRService {
    public OCRResultDTO? Process(in SKBitmap data, in Rect? area = null, in PageSegMode? pageSegMode = null) {
        var engine = enginePool.Get();
        try {
            using var pix = SkiaToPixConverter.ConvertBitmapToPix(data);

            using var page = area.HasValue ? engine.Process(pix, area.Value, pageSegMode) : engine.Process(pix, null, pageSegMode);
            if (page is null) return null;

            return new OCRResultDTO {
                Text = page.GetText(),
                Confidence = page.GetMeanConfidence()
            };
        } finally {
            enginePool.Return(engine);
        }
    }
}
