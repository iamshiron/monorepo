using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiGit.DB.Schema;

public enum Visibility {
    Public,
    Private,
    Internal
}

public enum Permission {
    Read,
    Write,
    Admin
}

public class Repository : BaseTable {
    [MaxLength(63)] public required string Name { get; set; }
    [MaxLength(255)] public string? Description { get; set; }
    public required Visibility Visibility { get; set; }

    public Guid OwnerID { get; set; }
    public User Owner { get; set; } = null!;
    [MaxLength(255)] public required string DefaultBranch { get; set; }
    [MaxLength(512)] public required string Location { get; set; }

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class RepositoryCollaborator {
    public Guid UserID { get; set; }
    public User User { get; set; } = null!;

    public Guid RepositoryID { get; set; }
    public Repository Repository { get; set; } = null!;

    public Permission Permission { get; set; }
}

public class LFSObject : BaseTable {
    [MaxLength(64)] public required string OID { get; set; }
    public required long SizeB { get; set; }
    public Guid RepositoryID { get; set; }
    public Repository Repository { get; set; } = null!;

    [MaxLength(255)] public required string BucketName { get; set; }
    [MaxLength(512)] public required string BucketKey { get; set; }

    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}
