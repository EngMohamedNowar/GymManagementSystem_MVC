using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Configurations
{
    public class MemberShipConfiguration : IEntityTypeConfiguration<MemberShip>
    {
        public void Configure(EntityTypeBuilder<MemberShip> builder)
        {
            builder.HasKey(K => K.Id);

            builder.HasOne(ms => ms.Member)
                .WithMany(m => m.Plans)
                .HasForeignKey(ms => ms.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ms => ms.Plan)
                .WithMany(p => p.Members)
                .HasForeignKey(ms => ms.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(P => P.Status)
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Active");

            builder.Property(P => P.CreatedAt)
                .HasColumnName("StartAt")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(P => P.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            builder.Property(P => P.DiscountCode)
                .HasMaxLength(50);
        }
    }
}
