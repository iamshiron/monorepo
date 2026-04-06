using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shiron.TheArchive.DB.Schema;

namespace Shiron.TheArchive.DB;

public class ArchiveDbContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options) {
    public DbSet<Character> Characters { get; set; }
    public DbSet<Media> Medias { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.Entity<Character>(entity => {
            entity.HasMany(c => c.Medias).WithMany(m => m.Characters);
            entity.HasMany(c => c.Images).WithOne(i => i.Character).HasForeignKey(i => i.CharacterID);
            entity.OwnsMany(c => c.Tags, t => t.ToJson());
            entity.OwnsMany(c => c.Alias, a => a.ToJson());
        });

        builder.Entity<Media>(entity => {
            entity.HasOne(m => m.WideBanner).WithOne().HasForeignKey<Media>(m => m.WideBannerID);
            entity.HasOne(m => m.SquareBanner).WithOne().HasForeignKey<Media>(m => m.SquareBannerID);
            entity.HasOne(m => m.Studio).WithMany(s => s.Medias).HasForeignKey(m => m.StudioID);
            entity.OwnsMany(m => m.Tags, t => t.ToJson());
        });

        builder.Entity<Image>(entity => {
            entity.OwnsOne(i => i.PrimaryColor);
            entity.OwnsOne(i => i.SecondaryColor);
            entity.OwnsMany(i => i.Palette, t => t.ToJson());
        });
    }
}
