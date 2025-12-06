namespace FiloShop.Application.Authentication;

public interface IUserContext
{
    Guid UserId { get; }

    string IdentityId { get; }
}