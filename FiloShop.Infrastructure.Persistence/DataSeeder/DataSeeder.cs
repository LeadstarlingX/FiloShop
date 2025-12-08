using System.Data;
using Bogus;
using Dapper;
using FiloShop.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FiloShop.Infrastructure.Persistence.DataSeeder;

// Seed Data Records
internal sealed record UserSeedData(
    Guid Id,
    string FirstName_Value,
    string LastName_Value,
    string Email_Value,
    string IdentityId,
    bool IsActive);

internal sealed record CatalogBrandSeedData(
    Guid Id,
    string Brand_Value);

internal sealed record CatalogTypeSeedData(
    Guid Id,
    string Type_Value);

internal sealed record CatalogItemSeedData(
    Guid Id,
    string Name_Value,
    string Description_Value,
    decimal Price_Amount,
    string Price_Currency,
    string PictureUri_Value,
    Guid CatalogTypeId,
    Guid CatalogBrandId);

internal sealed record BasketSeedData(
    Guid Id,
    Guid UserId);

internal sealed record BasketItemSeedData(
    Guid Id,
    decimal UnitPrice_Amount,
    string UnitPrice_Currency,
    int Quantity,
    Guid CatalogItemId,
    Guid BasketId);

public static class DataSeeder
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using var connection = sqlConnectionFactory.CreateConnection();

        SeedDataAsync(connection).GetAwaiter().GetResult();
    }

    private static async Task SeedDataAsync(IDbConnection connection)
    {
        // await ClearAllDataAsync(connection);

        var userIds = await SeedUsersAsync(connection);
        var brandIds = await SeedCatalogBrandsAsync(connection);
        var typeIds = await SeedCatalogTypesAsync(connection);
        var catalogItemIds = await SeedCatalogItemsAsync(connection, brandIds, typeIds);
        var basketIds = await SeedBasketsAsync(connection, userIds);
        await SeedBasketItemsAsync(connection, basketIds, catalogItemIds);
    }

    private static async Task ClearAllDataAsync(IDbConnection connection)
    {
        const string sql = """
                           TRUNCATE TABLE public."BasketItem" RESTART IDENTITY CASCADE;
                           TRUNCATE TABLE public."Basket" RESTART IDENTITY CASCADE;
                           TRUNCATE TABLE public."CatalogItem" RESTART IDENTITY CASCADE;
                           TRUNCATE TABLE public."CatalogBrand" RESTART IDENTITY CASCADE;
                           TRUNCATE TABLE public."CatalogType" RESTART IDENTITY CASCADE;
                           TRUNCATE TABLE public."User" RESTART IDENTITY CASCADE;
                           """;
        await connection.ExecuteAsync(sql);
    }

    private static async Task<List<Guid>> SeedUsersAsync(IDbConnection connection)
    {
        var faker = new Faker();
        var users = new List<UserSeedData>();
        var userIds = new List<Guid>();

        for (var i = 0; i < 10; i++)
        {
            var newId = Guid.NewGuid();
            userIds.Add(newId);

            users.Add(new UserSeedData(
                newId,
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Internet.Email(),
                string.Empty,
                true
            ));
        }

        const string sql = """
                           INSERT INTO public."User"
                           ("Id", "FirstName_Value", "LastName_Value", "Email_Value", "IdentityId", "IsActive")
                           VALUES(@Id, @FirstName_Value, @LastName_Value, @Email_Value, @IdentityId, @IsActive);
                           """;

        await connection.ExecuteAsync(sql, users);
        return userIds;
    }

    private static async Task<List<Guid>> SeedCatalogBrandsAsync(IDbConnection connection)
    {
        var faker = new Faker();
        var brands = new List<CatalogBrandSeedData>();
        var brandIds = new List<Guid>();

        var brandNames = new[] { "Nike", "Adidas", "Apple", "Samsung", "Sony", "Microsoft", "Dell", "HP", "Lenovo", "Asus" };

        foreach (var brandName in brandNames)
        {
            var newId = Guid.NewGuid();
            brandIds.Add(newId);

            brands.Add(new CatalogBrandSeedData(newId, brandName));
        }

        const string sql = """
                           INSERT INTO public."CatalogBrand"
                           ("Id", "Brand_Value")
                           VALUES(@Id, @Brand_Value);
                           """;

        await connection.ExecuteAsync(sql, brands);
        return brandIds;
    }

    private static async Task<List<Guid>> SeedCatalogTypesAsync(IDbConnection connection)
    {
        var types = new List<CatalogTypeSeedData>();
        var typeIds = new List<Guid>();

        var typeNames = new[] { "Electronics", "Clothing", "Footwear", "Accessories", "Sports", "Books", "Home & Garden", "Toys", "Food", "Health" };

        foreach (var typeName in typeNames)
        {
            var newId = Guid.NewGuid();
            typeIds.Add(newId);

            types.Add(new CatalogTypeSeedData(newId, typeName));
        }

        const string sql = """
                           INSERT INTO public."CatalogType"
                           ("Id", "Type_Value")
                           VALUES(@Id, @Type_Value);
                           """;

        await connection.ExecuteAsync(sql, types);
        return typeIds;
    }

    private static async Task<List<Guid>> SeedCatalogItemsAsync(IDbConnection connection, List<Guid> brandIds, List<Guid> typeIds)
    {
        var faker = new Faker();
        var items = new List<CatalogItemSeedData>();
        var itemIds = new List<Guid>();

        for (var i = 0; i < 50; i++)
        {
            var newId = Guid.NewGuid();
            itemIds.Add(newId);

            items.Add(new CatalogItemSeedData(
                newId,
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                decimal.Parse(faker.Commerce.Price(10, 1000)),
                "USD",
                faker.Image.PicsumUrl(),
                faker.PickRandom(typeIds),
                faker.PickRandom(brandIds)
            ));
        }

        const string sql = """
                           INSERT INTO public."CatalogItem"
                           ("Id", "Name_Value", "Description_Value", "Price_Amount", "Price_Currency", "PictureUri_Value", "CatalogTypeId", "CatalogBrandId")
                           VALUES(@Id, @Name_Value, @Description_Value, @Price_Amount, @Price_Currency, @PictureUri_Value, @CatalogTypeId, @CatalogBrandId);
                           """;

        await connection.ExecuteAsync(sql, items);
        return itemIds;
    }

    private static async Task<List<Guid>> SeedBasketsAsync(IDbConnection connection, List<Guid> userIds)
    {
        var baskets = new List<BasketSeedData>();
        var basketIds = new List<Guid>();

        // Create a basket for each user
        foreach (var userId in userIds)
        {
            var newId = Guid.NewGuid();
            basketIds.Add(newId);

            baskets.Add(new BasketSeedData(newId, userId));
        }

        const string sql = """
                           INSERT INTO public."Basket"
                           ("Id", "UserId")
                           VALUES(@Id, @UserId);
                           """;

        await connection.ExecuteAsync(sql, baskets);
        return basketIds;
    }

    private static async Task SeedBasketItemsAsync(IDbConnection connection, List<Guid> basketIds, List<Guid> catalogItemIds)
    {
        var faker = new Faker();
        var basketItems = new List<BasketItemSeedData>();

        // Add 2-5 random items to each basket
        foreach (var basketId in basketIds)
        {
            var itemCount = faker.Random.Int(2, 5);
            
            for (var i = 0; i < itemCount; i++)
            {
                var newId = Guid.NewGuid();

                basketItems.Add(new BasketItemSeedData(
                    newId,
                    decimal.Parse(faker.Commerce.Price(10, 500)),
                    "USD",
                    faker.Random.Int(1, 5),
                    faker.PickRandom(catalogItemIds),
                    basketId
                ));
            }
        }

        const string sql = """
                           INSERT INTO public."BasketItem"
                           ("Id", "UnitPrice_Amount", "UnitPrice_Currency", "Quantity", "CatalogItemId", "BasketId")
                           VALUES(@Id, @UnitPrice_Amount, @UnitPrice_Currency, @Quantity, @CatalogItemId, @BasketId);
                           """;

        await connection.ExecuteAsync(sql, basketItems);
    }
}