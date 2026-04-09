namespace Shiron.Mutils.API.DTos.API.Services;

public interface IExportService {
    string ExportToMudaeFormat(IEnumerable<string> characters);
    string ExportToCommaSeparated(IEnumerable<string> characters);
    string ExportToNewlineSeparated(IEnumerable<string> characters);
}
