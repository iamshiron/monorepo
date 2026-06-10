namespace Shiron.ResonanceSystem.API.Exceptions;

public sealed class ResonatorNameParseException : OCRException {
    public ResonatorNameParseException(string rawData)
        : base("Failed to parse resonator name.", rawData) { }
}
