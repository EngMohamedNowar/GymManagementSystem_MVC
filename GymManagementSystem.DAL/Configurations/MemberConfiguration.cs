using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Configurations
{
    public class MemberConfiguration :GymUserConfiguration<Member>,IEntityTypeConfiguration<Member>
    {
        public new void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(U => U.CreatedAt)
                .HasColumnName("JoinDate")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(U => U.HealthId).IsUnique();
            base.Configure(builder); 
        }
    }
}
