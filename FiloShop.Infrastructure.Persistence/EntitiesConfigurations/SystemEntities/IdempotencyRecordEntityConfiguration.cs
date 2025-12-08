using FiloShop.SharedKernel.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.SystemEntities;

public class IdempotencyRecordEntityConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.HasKey(x => x.IdempotencyKey);
        
        builder.Property(x => x.RequestName)
            .HasMaxLength(500)
            .IsRequired();
        
        builder.Property(x => x.SerializedResponse)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("ix_idempotency_records_created_at");
    }
}