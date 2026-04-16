using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Shiron.HonamiSystem.Schema;

public class User : IdentityUser<Guid> {
    [MaxLength(32)] public required string DisplayName { get; set; }
    [MaxLength(32)] public required override string? UserName { get; set; }

    [InverseProperty(nameof(Organization.Owner))]
    public IList<Organization> Organizations { get; set; } = [];
    public IList<UserSSHKey> SSHKeys { get; set; } = [];
    public IList<OrganizationMember> OrganizationMembers { get; set; } = [];
}

public class UserSSHKey : BaseEntity {
    [MaxLength(64)] public required string Name { get; set; } = string.Empty;
    [MaxLength(255)] public string Description { get; set; } = string.Empty;
    [MaxLength(16384)] public required string Key { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public required User User { get; set; }
    public required Guid UserID { get; set; }
}
