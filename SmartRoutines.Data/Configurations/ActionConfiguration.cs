using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Data.Configurations
{
    internal class ActionEntryConfiguration : IEntityTypeConfiguration<ActionEntry>
    {
        public void Configure(EntityTypeBuilder<ActionEntry> builder)
        {
            throw new NotImplementedException();
        }
    }
}
