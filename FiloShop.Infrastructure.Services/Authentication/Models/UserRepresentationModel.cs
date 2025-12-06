using FiloShop.Domain.Users.Entities;

namespace FiloShop.Infrastructure.Services.Authentication.Models;

internal sealed class UserRepresentationModel
{
    public Dictionary<string, string> Access { get; set; } = null!;

    public Dictionary<string, List<string>> Attributes { get; set; } = null!;

    public Dictionary<string, string> ClientRoles { get; set; }= null!;

    public long? CreatedTimestamp { get; set; }

    public CredentialRepresentationModel[] Credentials { get; set; } = null!;

    public string[] DisableableCredentialTypes { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool? EmailVerified { get; set; }

    public bool? Enabled { get; set; }

    public string FederationLink { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string[] Groups { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? NotBefore { get; set; }

    public string Origin { get; set; } = null!;

    public string[] RealmRoles { get; set; } = null!;

    public string[] RequiredActions { get; set; } = null!;

    public string Self { get; set; }= null!;

    public string ServiceAccountClientId { get; set; } = null!;

    public string Username { get; set; } = null!;

    internal static UserRepresentationModel FromUser(User user)
    {
        return new UserRepresentationModel
        {
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Email = user.Email.Value,
            Username = user.Email.Value,
            Enabled = true,
            EmailVerified = true,
            CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Attributes = new Dictionary<string, List<string>>(),
            RequiredActions = Array.Empty<string>()
        };
    }
}