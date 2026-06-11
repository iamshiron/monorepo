using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.DB;

public class ResSystemDbContext(DbContextOptions<ResSystemDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options) {
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<OwnedCharacter> OwnedCharacters => Set<OwnedCharacter>();
    public DbSet<OwnedEcho> OwnedEchos => Set<OwnedEcho>();
    public DbSet<EchoSubStat> EchoSubStats => Set<EchoSubStat>();
    public DbSet<EchoSonata> EchoSonatas => Set<EchoSonata>();
    public DbSet<Echo> Echoes => Set<Echo>();

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<Character>(entity => {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();
            entity.ToTable("Characters");
        });

        builder.Entity<OwnedCharacter>(entity => {
            entity.HasMany(c => c.Echoes)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OwnedEcho>(entity => {
            entity.HasMany(e => e.SubStats)
                .WithOne(s => s.Echo)
                .HasForeignKey(s => s.EchoID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<EchoSonata>(entity => {
            entity.HasMany(e => e.Echoes)
                .WithMany(s => s.Sonatas)
                .UsingEntity(j => j.ToTable("EchoSonataLinks"));
        });

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}
