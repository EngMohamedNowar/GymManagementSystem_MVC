using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Configurations
{
    public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.HasOne(h => h.Member)
                .WithOne(m => m.Health)
                .HasForeignKey<Member>(m => m.HealthId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
