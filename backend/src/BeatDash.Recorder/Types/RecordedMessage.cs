namespace Shiron.BeatDash.Recorder.Types;

public record RecordedMessage(DateTimeOffset Timestamp, string Endpoint, string Message);
