using Shiron.Mutils.Core.DTOs;

namespace Shiron.Mutils.Core.Services;

public interface IKakeraLogParser {
    IEnumerable<ParsedKakeraClaim> ParseKakeraLog(string data);
}
