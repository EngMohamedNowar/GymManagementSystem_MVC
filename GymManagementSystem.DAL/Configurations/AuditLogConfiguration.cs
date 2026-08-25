using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.ToTable("AuditLogs");

            builder.Property(a => a.UserName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(a => a.Action)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(a => a.Entity)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.EntityId)
                .HasMaxLength(100);

            builder.Property(a => a.Details)
                .HasMaxLength(1000);

            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
