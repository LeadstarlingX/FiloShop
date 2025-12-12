using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.CatalogItems.Errors;

public static class CatalogItemErrors
{
    public static readonly Error NotFound = new(
        "CatalogItem.NotFound", "CatalogItem not found");
    
    public static readonly Error Duplicated = new(
        "CatalogItem.Duplicated", "A CatalogItem with the same name already exists");

    public static readonly Error CatalogTypeNotFound = new(
        "CatalogItem.CatalogTypeNotFound", "The requested CatalogType was not found");

    public static readonly Error CatalogBrandNotFound = new(
        "CatalogItem.CatalogBrandNotFound", "The requested CatalogBrand was not found");
}