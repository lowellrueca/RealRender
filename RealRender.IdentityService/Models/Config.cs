using Duende.IdentityServer.Models;
namespace RealRender.IdentityService.Models;

public static class Config
{
    private const string _scope = "product-api-service";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource
            {
                Name = "product-api-service-resource",
                DisplayName = "Product Api",
                Scopes = new string[] {_scope}
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(name: _scope, displayName: "Product Api")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client{
                ClientId = GetClientId(),
                ClientName = "product-api",
                ClientSecrets = {new Secret(GetClientSecret())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {_scope},
                AllowedCorsOrigins = new string[] { "https://localhost:7001" }
            }
        };

    private static string GetClientId()
    {
        return Environment.GetEnvironmentVariable("CONFIG__CLIENT_ID").ToString().Sha512();
    }

    private static string GetClientSecret()
    {
        return Environment.GetEnvironmentVariable("CONFIG__CLIENT_SECRET").ToString().Sha512();
    }
}
