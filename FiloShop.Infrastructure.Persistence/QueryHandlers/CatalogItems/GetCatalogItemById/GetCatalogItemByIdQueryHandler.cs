using Dapper;
using FiloShop.Application.CatalogItems.Queries.GetCatalogItemById;
using FiloShop.Domain.CatalogItems.Errors;
using FiloShop.SharedKernel.CQRS.Queries;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Infrastructure.Persistence.QueryHandlers.CatalogItems.GetCatalogItemById;

public class GetCatalogItemByIdQueryHandler :
        IQueryHandler<GetCatalogItemByIdQuery, CatalogItemResponse>
{
        
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetCatalogItemByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
                _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<CatalogItemResponse>> Handle(GetCatalogItemByIdQuery request, CancellationToken cancellationToken)
        {
                using var connection = _sqlConnectionFactory.CreateConnection();

                const string sql = """

                                   """;

                var catalogItem = await connection.QueryFirstOrDefaultAsync<CatalogItemResponse>(
                        sql,
                        new
                        { 
                                request.CatalogItemId
                        }
                );

                if (catalogItem is null)
                        return Result.Failure<CatalogItemResponse>(CatalogItemErrors.NotFound);
                
                return catalogItem;
                
        }
}