using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Data.Configurations
{
    internal class RoutineConfiguration : IEntityTypeConfiguration<Routine>
    {
        public void Configure(EntityTypeBuilder<Routine> builder)
        {
            builder.ToTable("Routines");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(r => r.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(r => r.IconPath)
                .IsRequired()
                .HasMaxLength(500)
                .HasDefaultValue("default_icon.png");

            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(r => r.TriggerType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(r => r.TriggerConfig)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.UpdatedAt).IsRequired(false);
            builder.Property(r => r.IsDeleted).HasDefaultValue(false);


            // Ensure EF Core uses the private field for Actions
            builder.Metadata.FindNavigation(nameof(Routine.Actions))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Relationships 
            builder.HasMany(r => r.Actions)
                .WithOne(a => a.Routine)
                .HasForeignKey(a => a.RoutineId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(r => r.Name)
                .IsUnique()
                .HasDatabaseName("IX_Routine_Name");

            builder.HasIndex(r => r.IsActive);

        }
    }
}
