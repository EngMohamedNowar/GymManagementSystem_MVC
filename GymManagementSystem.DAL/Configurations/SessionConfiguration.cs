using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.HasIndex(s => new { s.StartDate, s.EndDate });

            builder.HasOne(s => s.Trainer)
                .WithMany(t => t.Sessions)
                .HasForeignKey(s => s.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Category)
                .WithMany()
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("SessionCapacityCheck", "Capacity Between 1 and 25");
                tb.HasCheckConstraint("SessionDateCheck", "EndDate > StartDate");
            });
        }
    }
}
