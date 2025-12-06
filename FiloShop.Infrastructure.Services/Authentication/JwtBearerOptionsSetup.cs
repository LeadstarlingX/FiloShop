using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace FiloShop.Infrastructure.Services.Authentication;

internal sealed class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authenticationOptions;

    public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
    {
        _authenticationOptions = authenticationOptions.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.Audience = _authenticationOptions.Audience;
        options.MetadataAddress = _authenticationOptions.MetaDataUrl;
        options.TokenValidationParameters.ValidIssuer = _authenticationOptions.Issuer;
        options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetadata;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}