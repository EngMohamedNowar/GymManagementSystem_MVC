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

            builder.Property(P => P.CreatedAt)
            .HasColumnName("BookingDate")
            .HasDefaultValueSql("GetDate()");

        }
    }
}
