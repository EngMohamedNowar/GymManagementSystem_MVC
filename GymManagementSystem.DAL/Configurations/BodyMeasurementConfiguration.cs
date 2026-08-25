using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations
{
    public class BodyMeasurementConfiguration : IEntityTypeConfiguration<BodyMeasurement>
    {
        public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
        {
            builder.HasKey(b => b.Id);
            builder.ToTable("BodyMeasurements");

            builder.Property(b => b.Weight)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.Height)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.BodyFat)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Notes)
                .HasMaxLength(500);

            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(b => b.Member)
                .WithMany()
                .HasForeignKey(b => b.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
