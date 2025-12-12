using Dapper;
using FiloShop.Application.CatalogBrands.Queries.GetCatalogBrands;
using FiloShop.SharedKernel.CQRS.Queries;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Infrastructure.Persistence.QueryHandlers.CatalogBrands.GetCatalogBrands;

internal sealed class GetCatalogBrandsQueryHandler : 
        IQueryHandler<GetCatalogBrandsQuery, IReadOnlyList<CatalogBrandResponse>>
{
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetCatalogBrandsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
                _sqlConnectionFactory = sqlConnectionFactory;
        }


        public async Task<Result<IReadOnlyList<CatalogBrandResponse>>> Handle(GetCatalogBrandsQuery request, CancellationToken cancellationToken)
        {
                using var connection = _sqlConnectionFactory.CreateConnection();

                const string sql = """
                                   
                                   """;
                
                var brands = await connection.QueryAsync<CatalogBrandResponse>(
                        sql
                );

                return brands.ToList();
        }
}