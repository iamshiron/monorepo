using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiGit.DB.Schema;

public class SSHKey : BaseTable {
    public Guid UserID { get; set; }
    public User User { get; set; } = null!;
    [MaxLength(64)] public required string Fingerprint { get; set; }
    [MaxLength(4096)] public required string PublicKey { get; set; }
}
