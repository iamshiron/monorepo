using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shiron.Lib.Types.EFCore;
using Shiron.TheArchive.DB.Schema;
using Shiron.TheArchive.DB.Schema.Car;

namespace Shiron.TheArchive.DB;

public class ArchiveDbContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options) {
    public DbSet<Character> Characters { get; set; }
    public DbSet<Media> Medias { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<Car> Cars { get; set; }

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
            entity.PrimitiveCollection(c => c.Tags);
            entity.PrimitiveCollection(c => c.Alias);
        });

        builder.Entity<Media>(entity => {
            entity.HasOne(m => m.WideBanner).WithOne().HasForeignKey<Media>(m => m.WideBannerID);
            entity.HasOne(m => m.SquareBanner).WithOne().HasForeignKey<Media>(m => m.SquareBannerID);
            entity.HasOne(m => m.Studio).WithMany(s => s.Medias).HasForeignKey(m => m.StudioID);
            entity.PrimitiveCollection(m => m.Tags);
        });

        builder.Entity<Brand>(entity => {
            entity.HasMany(b => b.Models).WithOne(m => m.Brand).HasForeignKey(m => m.BrandID);
            entity.HasMany(b => b.Cars).WithOne(c => c.Brand).HasForeignKey(c => c.BrandID);
        });

        builder.Entity<Model>(entity => {
            entity.HasMany(m => m.Cars).WithOne(c => c.Model).HasForeignKey(c => c.ModelID);
        });

        builder.Entity<Car>(entity => {
            entity.HasMany(c => c.Images).WithMany(i => i.Cars);
        });

        builder.Entity<Image>(entity => {
            entity.OwnsOne(i => i.PrimaryColor, c => {
                c.Property(p => p.Color).IsColor32();
                c.OwnsOne(p => p.Lab).OwnLabColor();
            });
            entity.OwnsOne(i => i.SecondaryColor, c => {
                c.Property(p => p.Color).IsColor32();
                c.OwnsOne(p => p.Lab).OwnLabColor();
            });
            entity.OwnsMany(i => i.Palette, c => {
                c.Property(p => p.Color).IsColor32();
                c.OwnsOne(p => p.Lab);
                c.ToJson();
            });
        });
    }
}
