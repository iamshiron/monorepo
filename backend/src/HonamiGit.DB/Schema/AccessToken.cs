using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiGit.DB.Schema;

public class AccessToken : BaseTable {
    public Guid UserID { get; set; }
    public User User { get; set; } = null!;

    [MaxLength(63)] public required string Name { get; set; }
    public required string Token { get; set; }
    public required List<string> Scopes { get; set; } = [];
    public DateTimeOffset? ExpiresAt { get; set; }
}
