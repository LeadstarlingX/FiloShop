using FiloShop.SharedKernel.Results;
using MediatR;

namespace FiloShop.SharedKernel.CQRS.Commands;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}

public interface IBaseCommand
{
}