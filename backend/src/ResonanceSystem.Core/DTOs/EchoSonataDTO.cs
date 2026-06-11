namespace Shiron.ResonanceSystem.Core.DTOs;

public record EchoSonataDTO {
    public required Guid ID { get; init; }
    public required string Name { get; init; }
    public required IList<EchoDTO> Echoes { get; init; } = [];
}
