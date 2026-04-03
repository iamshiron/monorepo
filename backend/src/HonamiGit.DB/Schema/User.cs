using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Shiron.HonamiGit.DB.Schema;

public class User : IdentityUser<Guid> {
    public override Guid Id { get; set; } = Guid.CreateVersion7();
    [MaxLength(63)] public required string DisplayName { get; set; }
    [MaxLength(63)] public override string? UserName { get; set; }
}
