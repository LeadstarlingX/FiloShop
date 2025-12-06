using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.Users.GetLoggedInUser;

public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;