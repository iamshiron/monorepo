using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiGit.DB.Schema;

public enum RepositoryVisibility {
    Public,
    Private,
    Unlisted
}

public enum RepositoryOwnerType {
    User,
    Organization
}

public class Repository : BaseEntity {
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(2047)] public string Description { get; set; } = string.Empty;
    [MaxLength(255)] public string? CachedBranch { get; set; }

    public RepositoryVisibility Visibility { get; set; }

    public required User CreatedBy { get; set; }
    public required Guid CreatedByID { get; set; }

    public RepositoryOwnerType OwnedByType { get; set; }
    public Guid OwnedByID { get; set; }
}

public enum ContributorAccess {
    Write,
    Read
}

public class Contributor {
    public required Guid UserID { get; set; }
    public required User User { get; set; }

    public required Guid RepositoryID { get; set; }
    public required Repository Repository { get; set; }

    public required ContributorAccess Access { get; set; }
}
