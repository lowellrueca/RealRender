using Duende.IdentityServer.Models;
namespace RealRender.IdentityService.Models;

public static class Config
{
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
                DisplayName = "Product Api Service Resource",
                Scopes = new string[] {"product-api-service"}
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(name: "product-api-service", displayName: "Product Api Service")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client{
                ClientId = "product-api-client",
                ClientSecrets = {new Secret("default-secret".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {"product-api-service"}
            }
        };
}
