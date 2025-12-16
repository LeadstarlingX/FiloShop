using FiloShop.SharedKernel.Resilience;

namespace FiloShop.SharedKernel.Interfaces;

public interface IDeadLetterQueueWriter
{
    Task WriteAsync(DeadLetterMessage message, CancellationToken cancellationToken = default);
}
