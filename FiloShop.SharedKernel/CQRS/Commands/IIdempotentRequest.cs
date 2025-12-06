namespace FiloShop.SharedKernel.CQRS.Commands;

public interface IIdempotentRequest : ICommand
{
    Guid IdempotencyKey { get; }
}

public interface IIdempotentRequest<TResponse> : ICommand<TResponse>
{
    Guid IdempotencyKey { get; }
}