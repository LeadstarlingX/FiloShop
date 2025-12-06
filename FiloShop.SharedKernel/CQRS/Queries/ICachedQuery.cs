namespace FiloShop.SharedKernel.CQRS.Queries;

public interface ICachedQuery<TRepsone> : IQuery<TRepsone>, ICachedQuery;

public interface ICachedQuery
{
    string CacheKey { get; }

    TimeSpan? Expiration { get; }
}