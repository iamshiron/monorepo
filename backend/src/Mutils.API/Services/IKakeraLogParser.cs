using Shiron.Mutils.API.DTOs;

namespace Shiron.Mutils.API.DTos.API.Services;

public interface IKakeraLogParser {
    IEnumerable<ParsedKakeraClaim> ParseKakeraLog(string data);
}
