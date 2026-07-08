using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace GymManagementSystem.DAL.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(N => N.Name)
                .HasMaxLength(20)
                .HasColumnType("varchar");

            builder.HasData(
                new Category { Id = 1, Name = "Cardio" },
                new Category { Id = 2, Name = "Strength" },
                new Category { Id = 3, Name = "Yoga" },
                new Category { Id = 4, Name = "Boxing" },
                new Category { Id = 5, Name = "CrossFit" }
);
        }
    }
}
