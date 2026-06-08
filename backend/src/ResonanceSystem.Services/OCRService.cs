using Microsoft.Extensions.ObjectPool;
using Shiron.ResonanceSystem.Core.DTOs;
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
    OCRResultDTO? Process(in byte[] data);
}

public class OCRService(ObjectPool<TesseractEngine> enginePool) : IOCRService {
    // public static async Task DownloadModel(string url, HttpClient? client = null) {
    //     client ??= new HttpClient();
    //     var outDir = Path.Combine(Directory.GetCurrentDirectory(), TESSDATA_DIR);
    //     if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
    //     var filePath = Path.Combine(outDir, Path.GetFileName(url));
    //     await using var stream = await client.GetStreamAsync(url);
    //     await using var fileStream = File.Create(filePath);
    //     await stream.CopyToAsync(fileStream);
    // }

    public OCRResultDTO? Process(in byte[] data) {
        var engine = enginePool.Get();
        try {
            using var pix = Pix.LoadFromMemory(data);
            using var page = engine.Process(pix);
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
