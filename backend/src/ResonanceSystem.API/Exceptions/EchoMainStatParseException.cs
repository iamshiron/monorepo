namespace Shiron.ResonanceSystem.API.Exceptions;

public sealed class EchoMainStatParseException : OCRException {
    public int EchoIndex { get; }

    public EchoMainStatParseException(int echoIndex, string rawData)
        : base($"Failed to parse echo main stat at index {echoIndex}.", rawData) {
        EchoIndex = echoIndex;
    }
}
