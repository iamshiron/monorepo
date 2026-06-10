namespace Shiron.ResonanceSystem.API.Exceptions;

public sealed class EchoSubStatParseException : OCRException {
    public int EchoIndex { get; }

    public EchoSubStatParseException(int echoIndex, string rawData)
        : base($"Failed to parse echo sub stats at index {echoIndex}.", rawData) {
        EchoIndex = echoIndex;
    }
}
