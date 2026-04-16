using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Shiron.HonamiSystem.DB.Schema;

public class User : IdentityUser<Guid> {
    [MaxLength(32)] public required string Name { get; set; }
    [MaxLength(32)] public override string? UserName { get; set; }

    public IList<Agent> Agents { get; set; } = [];
    public IList<Chat> Chats { get; set; } = [];
    public IList<ChatGroup> ChatGroups { get; set; } = [];
    public IList<Persona> Personas { get; set; } = [];
    public IList<Skill> Skills { get; set; } = [];
    public IList<ImageHandle> Images { get; set; } = [];
    public IList<FileHandle> Files { get; set; } = [];

    public Guid ID {
        get => Id;
        set => Id = value;
    }
}
