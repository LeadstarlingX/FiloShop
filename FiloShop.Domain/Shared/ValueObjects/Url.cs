using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record Url
{
    public string Value { get; }

    public Url(string value)
    {
        this.Value = value;
    }

    public static Result<Url> Create(string value)
    {
        return new Url(value);
    }
    
}