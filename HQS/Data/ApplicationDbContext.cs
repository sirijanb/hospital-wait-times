using HQS.Domain.Entities;
using HQS.Domain.Enums;
using HQS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HQS.Infrastructure.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Hospital> Hospitals => Set<Hospital>();
        public DbSet<HospitalRep> HospitalRepresentatives => Set<HospitalRep>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Hospital>(entity =>
            {
                entity.HasKey(h => h.HospitalId);
                entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
                entity.Property(h => h.Address).IsRequired();
                entity.Property(h => h.PostalCode).IsRequired().HasMaxLength(20);
            });

            builder.Entity<HospitalRep>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.Hospital)
                      .WithMany()
                      .HasForeignKey(r => r.HospitalId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
