using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(I => I.Id);
            builder.HasKey(B => new { B.SessionId, B.MemberId });

            builder.HasOne(b => b.Member)
                .WithMany(m => m.Sessions)
                .HasForeignKey(b => b.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Session)
                .WithMany(s => s.Members)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(P => P.CreatedAt)
            .HasColumnName("BookingDate")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        }
    }
}
