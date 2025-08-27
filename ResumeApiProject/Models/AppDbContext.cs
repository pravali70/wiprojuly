using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ResumeApi.Models
{
    //  IdentityDbContext<ApplicationUser, IdentityRole, string> gives us AspNetUsers + Roles
    public partial class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        //  Only keep your custom tables
        public virtual DbSet<Resume> Resumes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //  Important: Let Identity configure its own tables
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Resume>(entity =>
            {
                entity.HasKey(e => e.ResumeId);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Resumes)
                    .HasForeignKey(d => d.UserId)   // FK to ApplicationUser.Id (string)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ❌ Removed Admin + User (they were from your old schema)
        }
    }
}