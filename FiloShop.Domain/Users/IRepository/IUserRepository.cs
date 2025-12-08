using FiloShop.Domain.Users.Entities;
using FiloShop.SharedKernel.Interfaces;

namespace FiloShop.Domain.Users.IRepository;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(User user);

    Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default);

    Task<HashSet<string>> GetPermissionsAsync(string identityId, CancellationToken cancellationToken = default);
    
}