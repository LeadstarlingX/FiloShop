namespace FiloShop.Domain.Order.ValueObjects;

public sealed record Address(
    string Country,
    string Street,
    string City,
    string State,
    string ZipCode);