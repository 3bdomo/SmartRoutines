using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Data.Configurations
{
    internal class AppSettingsConfiguration : IEntityTypeConfiguration<AppSettings>
    {
        public void Configure(EntityTypeBuilder<AppSettings> builder)
        {
            builder.ToTable("AppSettings");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Theme)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Dark");

            builder.Property(s => s.DefaultLanguage)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("en-US");

            builder.Property(s => s.RunAtStartup)
                .HasDefaultValue(true);

            builder.Property(s => s.MinimizeToTray)
                .HasDefaultValue(true);

            builder.Property(s => s.ShowNotifications)
                .HasDefaultValue(true);

            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.UpdatedAt).IsRequired(false);
            builder.Property(s => s.IsDeleted).HasDefaultValue(false);


            // Seed initial data - only one row with Id = 1
            builder.HasData(new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Theme = "Dark",
                DefaultLanguage = "en-US",
                RunAtStartup = true,
                MinimizeToTray = true,
                ShowNotifications = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            });
        }

    }
}
