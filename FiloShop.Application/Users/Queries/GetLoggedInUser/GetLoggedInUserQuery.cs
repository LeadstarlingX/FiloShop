using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.Users.Queries.GetLoggedInUser;

public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;