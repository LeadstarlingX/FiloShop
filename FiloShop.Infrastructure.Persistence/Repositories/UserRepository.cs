using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FiloShop.Domain.Users.Entities;
using FiloShop.Domain.Users.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FiloShop.Infrastructure.Persistence.Repositories;

public class UserRepository : RepositoryBase<User> , IUserRepository
{
    public UserRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public UserRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
    {
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Add(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<HashSet<string>> GetPermissionsAsync(string identityId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}