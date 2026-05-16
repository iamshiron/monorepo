using Shiron.Mutils.API.DTOs;

namespace Shiron.Mutils.API.Services;

public interface IKakeraLogParser {
    IEnumerable<ParsedKakeraClaim> ParseKakeraLog(string data, int? timezoneOffsetMinutes = null);
}
