using System.ComponentModel.DataAnnotations;

namespace Shiron.TheArchive.DB.Schema;

public enum Gender {
    Male,
    Female,
    Other,
    Unknown
}

public class Character : BaseEntity, IOwned, ITaggable {
    [MaxLength(255)] public required string Name { get; set; }
    public IList<string> Alias { get; set; } = [];
    public Gender Gender { get; set; }
    public DateOnly Birthday { get; set; }

    public User? CreatedBy { get; set; }
    public Guid? CreatedByID { get; set; }

    public IList<string> Tags { get; set; } = [];

    public IList<Media> Medias { get; set; } = [];
    public IList<Image> Images { get; set; } = [];
}
