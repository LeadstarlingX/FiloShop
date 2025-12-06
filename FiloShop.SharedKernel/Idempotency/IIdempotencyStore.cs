namespace FiloShop.SharedKernel.Idempotency;

public interface IIdempotencyStore
{
    Task<IdempotencyRecord?> GetByKeyAsync(Guid idempotencyKey, CancellationToken cancellationToken = default);
    Task SaveAsync(IdempotencyRecord record, CancellationToken cancellationToken = default);
}