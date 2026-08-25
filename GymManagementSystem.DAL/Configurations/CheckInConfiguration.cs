using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations
{
    public class CheckInConfiguration : IEntityTypeConfiguration<CheckIn>
    {
        public void Configure(EntityTypeBuilder<CheckIn> builder)
        {
            builder.HasKey(c => c.Id);
            builder.ToTable("CheckIns");

            builder.Property(c => c.Note)
                .HasMaxLength(500);

            builder.Property(c => c.CheckInTime)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(c => c.Member)
                .WithMany()
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
