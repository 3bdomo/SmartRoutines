using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;

namespace SmartRoutines.Data.Configurations
{
    internal class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.RoutineId)
                .IsRequired();

            builder.Property(l => l.RoutineName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(l => l.Status)
                .IsRequired()
                .HasDefaultValue(LogStatus.Unknown)
                .HasConversion<int>();

            builder.Property(l => l.Message)
                .IsRequired()
                .HasMaxLength(1000)
                .HasDefaultValue(string.Empty);


            builder.Property(l => l.CreatedAt).IsRequired();
            builder.Property(l => l.UpdatedAt).IsRequired(false);
            builder.Property(l => l.IsDeleted).HasDefaultValue(false);

            // indexes
            builder.HasIndex(l => l.CreatedAt)
                .HasDatabaseName("IX_ActivityLog_CreatedAt");

            builder.HasIndex(l => l.Status)
                .HasDatabaseName("IX_ActivityLog_Status");

            // سرعة جلب تاريخ تنفيذ روتين معين
            builder.HasIndex(l => l.RoutineId)
                .HasDatabaseName("IX_ActivityLog_RoutineId");
        }
    }
}
