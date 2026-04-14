using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Data.Configurations
{
    internal class ActionEntryConfiguration : IEntityTypeConfiguration<ActionEntry>
    {
        public void Configure(EntityTypeBuilder<ActionEntry> builder)
        {
            builder.ToTable("ActionEntries");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.Arguments)
                .IsRequired()
                .HasMaxLength(1000)
                .HasColumnType("TEXT");

            builder.Property(a => a.ExecutionOrder)
                .IsRequired();

            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.UpdatedAt).IsRequired(false);
            builder.Property(a => a.IsDeleted).HasDefaultValue(false);

            builder.Property(a => a.RoutineId)
                .IsRequired();


            // Relationships
            builder.HasOne(a => a.Routine)
                .WithMany(r => r.Actions)
                .HasForeignKey(a => a.RoutineId)
                .OnDelete(DeleteBehavior.Cascade);

            // indexes
            builder.HasIndex(a => new { a.RoutineId, a.ExecutionOrder })
                .HasDatabaseName("IX_ActionEntry_RoutineId_Order");

            builder.HasIndex(a => a.Type);
        }
    }
}
