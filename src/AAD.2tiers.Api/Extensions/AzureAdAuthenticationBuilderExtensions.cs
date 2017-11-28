using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddAzureAdBearer(this AuthenticationBuilder builder)
            => builder.AddAzureAdBearer(_ => { });

        public static AuthenticationBuilder AddAzureAdBearer(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureAzureOptions>();
            builder.AddJwtBearer();
            return builder;
        }

        private class ConfigureAzureOptions: IConfigureNamedOptions<JwtBearerOptions>
        {
            private readonly AzureAdOptions _azureOptions;

            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions)
            {
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, JwtBearerOptions options)
            {
                //change here to different string comparision if do not want to use GUIDs
                options.Audience = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}/v2.0"; //for V2.0 endpoint
            }

            public void Configure(JwtBearerOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
