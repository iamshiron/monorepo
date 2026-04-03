using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiGit.DB.Schema;

namespace Shiron.HonamiGit.DB;

public class HonamiGitDb(DbContextOptions<HonamiGitDb> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options) {
    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<RepositoryCollaborator> RepositoryCollaborators => Set<RepositoryCollaborator>();
    public DbSet<LFSObject> LFSObjects => Set<LFSObject>();
    public DbSet<AccessToken> AccessTokens => Set<AccessToken>();
    public DbSet<SSHKey> SSHKeys => Set<SSHKey>();

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.Entity<Repository>(e => {
            e.ToTable("Repositories");
            e.HasKey(r => r.ID);
            e.HasOne(r => r.Owner)
                .WithMany()
                .HasForeignKey(r => r.OwnerID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(r => new { r.OwnerID, r.Name }).IsUnique();
        });

        builder.Entity<RepositoryCollaborator>(e => {
            e.ToTable("RepositoryCollaborators");
            e.HasKey(rc => new { rc.UserID, rc.RepositoryID });
            e.HasOne(rc => rc.User)
                .WithMany()
                .HasForeignKey(rc => rc.UserID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(rc => rc.Repository)
                .WithMany()
                .HasForeignKey(rc => rc.RepositoryID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<LFSObject>(e => {
            e.ToTable("LFSObjects");
            e.HasKey(l => l.ID);
            e.HasOne(l => l.Repository)
                .WithMany()
                .HasForeignKey(l => l.RepositoryID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(l => new { l.OID, l.RepositoryID }).IsUnique();
        });

        builder.Entity<AccessToken>(e => {
            e.ToTable("AccessTokens");
            e.HasKey(a => a.ID);
            e.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SSHKey>(e => {
            e.ToTable("SSHKeys");
            e.HasKey(s => s.ID);
            e.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(s => s.Fingerprint).IsUnique();
        });
    }
}
