using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(d => d.Id);
            builder.ToTable("Discounts");

            builder.Property(d => d.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(d => d.Code)
                .IsUnique();

            builder.Property(d => d.Type)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(d => d.Value)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.Description)
                .HasMaxLength(500);

            builder.Property(d => d.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
