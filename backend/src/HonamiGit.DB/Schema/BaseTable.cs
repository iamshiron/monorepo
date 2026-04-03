namespace Shiron.HonamiGit.DB.Schema;

public class BaseTable {
    public Guid ID { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
