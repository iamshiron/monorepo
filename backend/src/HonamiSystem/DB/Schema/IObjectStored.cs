namespace Shiron.HonamiSystem.DB.Schema;

public interface IObjectStored {
    string ObjectKey { get; set; }
    decimal SizeKb { get; set; }
}
