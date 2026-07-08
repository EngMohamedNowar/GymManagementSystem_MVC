using GymManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name)
                .HasColumnType("varchar")
                .HasMaxLength(128);

            builder.Property(p => p.Description)
                .HasMaxLength(200)
                .HasColumnType("varchar");

            builder.Property(p => p.Price)
                .HasPrecision(18, 3);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GetDate()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("PlanDurationDaysCheck", "DurationDays Between 1 and 356");
            });

        }
    }
}
