using FiloShop.Infrastructure.Persistence.AppDbContext;
using FiloShop.SharedKernel.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace FiloShop.Infrastructure.Persistence.Idempotency;

public sealed class IdempotencyStore : IIdempotencyStore
{
    private readonly ApplicationDbContext _context;
    public IdempotencyStore(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IdempotencyRecord?> GetByKeyAsync(
        Guid idempotencyKey, 
        CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<IdempotencyRecord>()
            .FirstOrDefaultAsync(
                r => r.IdempotencyKey == idempotencyKey, 
                cancellationToken);
    }
    public async Task SaveAsync(
        IdempotencyRecord record, 
        CancellationToken cancellationToken = default)
    {
        await _context.Set<IdempotencyRecord>().AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}