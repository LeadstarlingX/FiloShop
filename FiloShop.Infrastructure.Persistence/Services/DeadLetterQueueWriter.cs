using Dapper;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Resilience;

namespace FiloShop.Infrastructure.Persistence.Services;

public sealed class DeadLetterQueueWriter : IDeadLetterQueueWriter
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public DeadLetterQueueWriter(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task WriteAsync(DeadLetterMessage message, CancellationToken cancellationToken = default)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = @"
            INSERT INTO dead_letter_messages (id, type, content, error, occurred_on_utc)
            VALUES (@Id, @Type, @Content::jsonb, @Error, @OccurredOnUtc)";

        await connection.ExecuteAsync(sql, new
        {
            message.Id,
            message.Type,
            message.Content,
            message.Error,
            message.OccurredOnUtc
        });
    }
}
