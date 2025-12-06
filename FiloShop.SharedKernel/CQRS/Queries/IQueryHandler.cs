using FiloShop.SharedKernel.Results;
using MediatR;

namespace FiloShop.SharedKernel.CQRS.Queries;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}