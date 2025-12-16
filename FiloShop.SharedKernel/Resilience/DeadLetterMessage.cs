namespace FiloShop.SharedKernel.Resilience;

public sealed class DeadLetterMessage
{
    public DeadLetterMessage(Guid id, DateTime occurredOnUtc, string type, string content, string error)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Content = content;
        Error = error;
    }
    
    // For EF Core
    private DeadLetterMessage() { }

    public Guid Id { get; private set; }
    public string Type { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public string Error { get; private set; } = default!;
    public DateTime OccurredOnUtc { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? ProcessingError { get; private set; }

    public void MarkAsProcessed()
    {
        ProcessedOnUtc = DateTime.UtcNow;
        ProcessingError = null;
    }

    public void MarkAsFailed(string error)
    {
        ProcessingError = error;
    }
}
