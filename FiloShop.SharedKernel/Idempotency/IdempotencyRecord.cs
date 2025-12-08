using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.SharedKernel.Idempotency;

public sealed class IdempotencyRecord
{
    private IdempotencyRecord() { }

    private IdempotencyRecord(
        Guid idempotencyKey,
        string requestName,
        string serializedResponse)
    {
        IdempotencyKey = idempotencyKey;
        RequestName = requestName;
        SerializedResponse = serializedResponse;
        CreatedAt = DateTime.UtcNow;
    }

    public static IdempotencyRecord Create(
        Guid idempotencyKey,
        string requestName,
        string serializedResponse)
    {
        return new IdempotencyRecord(idempotencyKey, requestName, serializedResponse);
    }

    public Guid IdempotencyKey { get; private init; }
    public string RequestName { get; private init; } = string.Empty;
    public string SerializedResponse { get; private init; } = string.Empty;
    public DateTime CreatedAt { get; private init; }
}