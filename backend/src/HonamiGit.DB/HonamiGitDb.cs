using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiGit.DB.Schema;

namespace Shiron.HonamiGit.DB;

public class HonamiGitDb(DbContextOptions options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options) {
    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public DbSet<Contributor> Contributors => Set<Contributor>();
    public DbSet<UserSSHKey> UserSSHKeys => Set<UserSSHKey>();

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.Entity<Repository>(r => {
            r.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedByID)
                .OnDelete(DeleteBehavior.Restrict);

            r.HasIndex(e => new { e.OwnedByID, e.OwnedByType, e.Name }).IsUnique();
        });

        builder.Entity<Organization>(o => {
            o.HasOne(e => e.Owner)
                .WithMany(e => e.Organizations)
                .HasForeignKey(e => e.OwnerID)
                .OnDelete(DeleteBehavior.Restrict);

            o.HasIndex(e => e.Name).IsUnique();
        });

        builder.Entity<UserSSHKey>(s => {
            s.HasOne(e => e.User)
                .WithMany(e => e.SSHKeys)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrganizationMember>(c => {
            c.HasKey(e => new { e.MemberID, e.OrganizationID });

            c.HasOne(e => e.Member)
                .WithMany(e => e.OrganizationMembers)
                .HasForeignKey(e => e.MemberID)
                .OnDelete(DeleteBehavior.Cascade);

            c.HasOne(e => e.Organization)
                .WithMany(e => e.Members)
                .HasForeignKey(e => e.OrganizationID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Contributor>(c => {
            c.HasKey(e => new { e.UserID, e.RepositoryID });

            c.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            c.HasOne(e => e.Repository)
                .WithMany()
                .HasForeignKey(e => e.RepositoryID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
