namespace FiloShop.Infrastructure.Services.Authentication.Models;

internal class CredentialRepresentationModel
{
    public string Algorithm { get; set; } = null!;

    public string Config { get; set; } = null!;

    public int Counter { get; set; }

    public long CreatedDate { get; set; }

    public string Device { get; set; } = null!;

    public int Digits { get; set; }

    public int HashIterations { get; set; }

    public string HashedSaltedValue { get; set; } = null!;

    public int Period { get; set; }

    public string Salt { get; set; } = null!;

    public bool Temporary { get; set; }

    public string Type { get; set; } = null!;

    public string Value { get; set; } = null!;
}