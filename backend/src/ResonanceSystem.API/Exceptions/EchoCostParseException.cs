namespace Shiron.ResonanceSystem.API.Exceptions;

public sealed class EchoCostParseException : OCRException {
    public int EchoIndex { get; }

    public EchoCostParseException(int echoIndex, string rawData)
        : base($"Failed to parse echo cost at index {echoIndex}.", rawData) {
        EchoIndex = echoIndex;
    }
}
