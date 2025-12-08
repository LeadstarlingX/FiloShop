using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FiloShop.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditableEntityInterceptor(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            var now = _dateTimeProvider.UtcNow;

            if (entry.State == EntityState.Added)
            {
                SetPropertyValue(entity, nameof(BaseEntity.CreatedAt), now);
            }

            if (entry.State == EntityState.Modified)
            {
                SetPropertyValue(entity, nameof(BaseEntity.UpdatedAt), now);
            }
        }
    }

    private static void SetPropertyValue(BaseEntity entity, string propertyName, DateTime value)
    {
        var property = typeof(BaseEntity).GetProperty(propertyName);
        property?.SetValue(entity, value);
    }
}
