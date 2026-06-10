namespace Shiron.ResonanceSystem.API.Exceptions;

public abstract class OCRException : Exception {
    public string RawData { get; }

    protected OCRException(string message, string rawData) : base(message) {
        RawData = rawData;
    }

    protected OCRException(string message, string rawData, Exception innerException)
        : base(message, innerException) {
        RawData = rawData;
    }
}
