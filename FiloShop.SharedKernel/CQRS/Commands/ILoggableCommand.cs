namespace FiloShop.SharedKernel.CQRS.Commands;

public interface ILoggableCommand
{
    IDictionary<string, object> GetLoggingProperties();
}

public interface ILoggableCommand<TResponse> : ICommand<TResponse>, ILoggableCommand
{
}