using HQS.Domain.Entities;
using HQS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HQS.Infrastructure.Data.Configurations;

public class HQS : IEntityTypeConfiguration<Hospital>
{
    public void Configure(EntityTypeBuilder<Hospital> builder)
    {
        builder.HasKey(h => h.HospitalId);

        builder.Property(h => h.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(h => h.Address)
               .IsRequired()
               .HasMaxLength(300);

        builder.Property(h => h.ServicesOffered)
               .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => Enum.Parse<ServiceType>(s))
                          .ToList()
                )
               .HasColumnName("ServicesOffered")
               .HasMaxLength(200);

        builder.Property(h => h.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");
    }
}
