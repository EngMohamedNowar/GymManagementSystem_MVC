using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            builder.ToTable("Notifications");

            builder.Property(n => n.UserId)
                .HasMaxLength(450);

            builder.Property(n => n.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(n => n.Message)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(n => n.Type)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(n => n.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
