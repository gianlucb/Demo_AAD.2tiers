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
                //enable the access token request
                options.ResponseType = "token id_token";

                //the target resource URI (access token for) == Audience Uri (aud) in the JWT
                //note: by default the template wants this field equal to the target application Application ID (== AppId)
                //the target application will check this field in order to understand if the token is meant for itself
                //this check can be modified in order to use more meaningful string editing ConfigureAzureOptions in the target app (Api)
                options.Resource = _azureOptions.TargetApiAppId;

                //save tokens in the request context
                options.SaveTokens = true;

                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = false
                }; //to allow everybody , otherwise I need to set the list of tenants

            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
