using FiloShop.SharedKernel.Results;
using MediatR;

namespace FiloShop.SharedKernel.CQRS.Queries;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}