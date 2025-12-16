using FiloShop.SharedKernel.Resilience;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.SystemEntities;

internal sealed class DeadLetterMessageConfiguration : IEntityTypeConfiguration<DeadLetterMessage>
{
    public void Configure(EntityTypeBuilder<DeadLetterMessage> builder)
    {

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(x => x.Error)
            .IsRequired();

        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc);
        
        builder.Property(x => x.ProcessingError);

        builder.HasIndex(x => x.ProcessedOnUtc)
            .HasFilter("\"ProcessedOnUtc\" IS NULL");
    }
}
