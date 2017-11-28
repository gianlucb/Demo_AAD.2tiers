using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdAuthenticationBuilderExtensions
    {        
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        private class ConfigureAzureOptions: IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdOptions _azureOptions;
           
            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions)
            {
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                //in order to create the app registration we need to use the portal at https://apps.dev.microsoft.com/portal/application/
                //not the Azure portal. By the way the app will be registered in our Azure tenant


                //enable the access token request
                options.ResponseType = "token id_token";

                //not supported by V2.0
                //---------------------------------
                //the target resource URI (access token for) == Audience Uri (aud) in the JWT
                //note: by default the template wants this field equal to the target application Application ID (== AppId)
                //the target application will check this field in order to understand if the token is meant for itself
                //this check can be modified in order to use more meaningful string editing ConfigureAzureOptions in the target app (Api)                

                //options.Resource = _azureOptions.TargetApiAppId;
                //---------------------------------

                //save tokens in the request context
                options.SaveTokens = true;

                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}/v2.0"; //for V2.0 endpoint
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;

                //custom scopes defined in application manifest (via the new portal)
                //the scope name must have the resource ID as prefix
                //the scopes we check in the portal are the "default ones" that we can avoid to ask for
                //a dynamic consent page is created on the fly each time we request a scope not used before
                //in the V1.0 version the scopes are instead selected manually by the admin and are static - so cannot be requested by the client app
                options.Scope.Add("api://aad.2tiers.api/access_as_user");
                options.Scope.Add("api://aad.2tiers.api/aad.scopeA");
                //options.Scope.Add("api://aad.2tiers.api/aad.scopeB");

                //not supported by V2.0
                options.Scope.Remove("profile");                

            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
