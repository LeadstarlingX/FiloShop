namespace FiloShop.Domain.Orders.ValueObjects;

public sealed record Address(
    string Country,
    string Street,
    string City,
    string State,
    string ZipCode);