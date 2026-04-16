using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiSystem.DB.Schema;
using Shiron.Lib.Types;
using Shiron.Lib.Types.EFCore;

namespace Shiron.HonamiSystem.DB;

public class HonamiSystemDb(DbContextOptions options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options) {
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatGroup> ChatGroups => Set<ChatGroup>();
    public DbSet<ChatParticipantUser> ChatParticipants => Set<ChatParticipantUser>();
    public DbSet<ChatParticipantAgent> ChatParticipantAgents => Set<ChatParticipantAgent>();
    public DbSet<Memory> Memories => Set<Memory>();
    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<ImageHandle> Images => Set<ImageHandle>();
    public DbSet<FileHandle> Files => Set<FileHandle>();
    public DbSet<WidgetHandle> Widgets => Set<WidgetHandle>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.Entity<User>(e => {
            e.Ignore(u => u.ID);
        });

        builder.Entity<Agent>(e => {
            e.HasOne(a => a.Persona)
                .WithMany()
                .HasForeignKey(a => a.PersonaID)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(a => a.CreatedBy)
                .WithMany(u => u.Agents)
                .HasForeignKey(a => a.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);
            e.PrimitiveCollection(a => a.RequiredTools);
            e.PrimitiveCollection(a => a.SuggestedTools);
        });

        builder.Entity<Chat>(e => {
            e.HasOne(c => c.ChatGroup)
                .WithMany(g => g.Chats)
                .HasForeignKey(c => c.ChatGroupID)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.CreatedBy)
                .WithMany(u => u.Chats)
                .HasForeignKey(c => c.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);
            e.Ignore(c => c.Attachments);
        });

        builder.Entity<ChatGroup>(e => {
            e.HasOne(g => g.CreatedBy)
                .WithMany(u => u.ChatGroups)
                .HasForeignKey(g => g.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ChatMessage>(e => {
            e.HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Agent)
                .WithMany()
                .HasForeignKey(m => m.AgentID)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(m => m.ParentMessage)
                .WithMany()
                .HasForeignKey(m => m.ParentMessageID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ChatParticipantUser>(e => {
            e.HasKey(p => new { p.UserID, p.ChatID });
            e.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Chat)
                .WithMany(c => c.UserParticipants)
                .HasForeignKey(p => p.ChatID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ChatParticipantAgent>(e => {
            e.HasKey(p => new { p.AgentID, p.ChatID });
            e.HasOne(p => p.Agent)
                .WithMany()
                .HasForeignKey(p => p.AgentID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Chat)
                .WithMany(c => c.AgentParticipants)
                .HasForeignKey(p => p.ChatID)
                .OnDelete(DeleteBehavior.Cascade);
            e.PrimitiveCollection(p => p.AllowedTools);
        });

        builder.Entity<FileHandle>(e => {
            e.HasOne(f => f.CreatedBy)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(f => f.Message)
                .WithMany()
                .HasForeignKey(f => f.MessageID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ImageHandle>(e => {
            e.HasOne(i => i.CreatedBy)
                .WithMany(u => u.Images)
                .HasForeignKey(i => i.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(i => i.Message)
                .WithMany()
                .HasForeignKey(i => i.MessageID)
                .OnDelete(DeleteBehavior.Cascade);

            e.OwnsOne(i => i.PrimaryColor, c => {
                c.Property(p => p.Color).IsColor32();
                c.OwnsOne(p => p.Lab).OwnLabColor();
            });
            e.OwnsOne(i => i.SecondaryColor, c => {
                c.Property(p => p.Color).IsColor32();
                c.OwnsOne(p => p.Lab).OwnLabColor();
            });
            e.OwnsMany(i => i.Palette, c => {
                c.Property(p => p.Color).IsColor32();
                c.OwnsOne(p => p.Lab);
                c.ToJson();
            });
        });

        builder.Entity<WidgetHandle>(e => {
            e.HasOne(w => w.Message)
                .WithMany()
                .HasForeignKey(w => w.MessageID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.HasPostgresExtension("vector");

        builder.Entity<Memory>(e => {
            e.Property(m => m.Embedding).HasColumnType("vector(1536)");
            e.HasOne(m => m.Agent)
                .WithMany(a => a.Memories)
                .HasForeignKey(m => m.AgentID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Chat)
                .WithMany(c => c.Memories)
                .HasForeignKey(m => m.ChatID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Persona>(e => {
            e.HasOne(p => p.CreatedBy)
                .WithMany(u => u.Personas)
                .HasForeignKey(p => p.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);
            e.PrimitiveCollection(p => p.Traits);
        });

        builder.Entity<Skill>(e => {
            e.HasOne(s => s.CreatedBy)
                .WithMany(u => u.Skills)
                .HasForeignKey(s => s.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MessageAttachment>(e => {
            e.HasOne(ma => ma.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(ma => ma.MessageID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ma => ma.FileHandle)
                .WithMany()
                .HasForeignKey(ma => ma.FileHandleID)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(ma => ma.ImageHandle)
                .WithMany()
                .HasForeignKey(ma => ma.ImageHandleID)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(ma => ma.WidgetHandle)
                .WithMany()
                .HasForeignKey(ma => ma.WidgetHandleID)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
