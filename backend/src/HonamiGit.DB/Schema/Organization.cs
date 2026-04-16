using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.Schema;

public class Organization : BaseEntity {
    [MaxLength(32)] public required string Name { get; set; }
    [MaxLength(2047)] public string Description { get; set; } = string.Empty;

    public required Guid OwnerID { get; set; }
    public required User Owner { get; set; }

    public IList<OrganizationMember> Members { get; set; } = [];
    public IList<Repository> Repositories { get; set; } = [];
}

public enum OrganizationRole {
    CoOwner,
    Admin,
    Member
}

public class OrganizationMember {
    public required Guid OrganizationID { get; set; }
    public required Organization Organization { get; set; }

    public required User Member { get; set; }
    public required Guid MemberID { get; set; }

    public OrganizationRole Role { get; set; }
}
