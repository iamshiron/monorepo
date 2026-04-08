using System.ComponentModel.DataAnnotations;

namespace Shiron.TheArchive.DB.Schema;

public class Studio : BaseEntity, IOwned {
    [MaxLength(255)] public required string Name { get; set; }

    public User? CreatedBy { get; set; }
    public Guid? CreatedByID { get; set; }

    public IList<Media> Medias { get; set; } = [];
}

public enum MediaStatus {
    Completed,
    InProgress,
    Planned,
    Cancelled
}

public class Media : BaseEntity, IOwned, ITaggable {
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(2047)] public string Synopsis { get; set; } = string.Empty;
    public int EpisodeCount { get; set; }

    public Image? WideBanner { get; set; }
    public Guid? WideBannerID { get; set; }
    public Image? SquareBanner { get; set; }
    public Guid? SquareBannerID { get; set; }
    public MediaStatus Status { get; set; }

    public Studio? Studio { get; set; }
    public Guid? StudioID { get; set; }

    public User? CreatedBy { get; set; }
    public Guid? CreatedByID { get; set; }

    public List<string> Tags { get; set; } = [];

    public IList<Character> Characters { get; set; } = [];
}
