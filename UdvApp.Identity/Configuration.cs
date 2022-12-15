using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using static System.Net.WebRequestMethods;

namespace UdvApp.Identity
{
    public class Configuration
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("UdvAppWebAPI", "Web API"),
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource> 
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("UdvAppWebAPI", "Web API", new [] { JwtClaimTypes.Name, JwtClaimTypes.Id })
                {
                    Scopes = { "UdvAppWebAPI" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client> 
            {
                new Client()
                {
                    ClientId = "udvapp-web-api",
                    ClientName = "UdvApp Web",
                    AllowedGrantTypes =
                    {
                        GrantType.AuthorizationCode,
                        GrantType.ResourceOwnerPassword
                    },
                    RequireClientSecret = false,
                    RequirePkce = true,
                    
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "UdvAppWebAPI",
                    },

                    RedirectUris =
                    {
                        "https://localhost:7079/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:7079/signout-oidc"
                    },
                    AllowedCorsOrigins =
                    {
                        "https://localhost:7079",
                        "https://localhost:7058",
                    }
                    
                },
            };
    }
}
