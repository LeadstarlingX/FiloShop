using FiloShop.Domain.Users.Events;
using FiloShop.SharedKernel.Entities;

namespace FiloShop.Domain.Users.Entities;

public sealed class User : BaseEntity
{
    private User()
    {
    }

    public static User CreateInstance()
    {
        return new User();
    }
}