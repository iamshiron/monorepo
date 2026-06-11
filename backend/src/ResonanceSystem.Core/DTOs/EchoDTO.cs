using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core.DTOs;

public record EchoDTO {
    public required Guid ID { get; init; }
    public required string Name { get; init; }
    public required EchoCost Cost { get; init; }
}
