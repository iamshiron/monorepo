namespace Shiron.TheArchive.DB.Schema;

public class BaseEntity {
    public Guid ID { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
