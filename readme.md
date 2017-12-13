# Work in progress...

# Azure AD Secured Api access (Asp.Net Core 2.0) - Scopes - V2.0 endpoint

Small example of a two tiers application with a FrontEnd application that calls a backend Api application. Both coded with **Asp.Net Core 2.0** and secured with the same Azure Ad tenant (directory).
This demonstrate how to retrieve an access token from AzureAd, using the implicit flow (**OpenIdConnect**) and custom scopes:

![scenario](images/simple-AAD.png)

## Document
For more detailed information read the following [post](https://blogs.msdn.microsoft.com/gianlucb/2017/10/04/access-an-azure-ad-secured-api-with-asp-net-core-2-0/?preview_id=255&preview_nonce=7228ed3d1b&_thumbnail_id=265&preview=true)


## Setup
Using the new [Application registration portal](https://apps.dev.microsoft.com/portal/application) you need to:
+ create two Azure Applications in your tenant
    + FrontEnd (Web platform)
        + Reply Url: [http://localhost:10001/signin-oidc](#)    
        + Allow Implict Flow
    + Api (Api platform)
        + Reply Url: [http://localhost:10000](#)
        + App ID Uri: [api://aad.2tiers.api](#)
        + Add two custom scopes: **aad.scopeA** and **aad.scopeB**

+ allows the FrontEnd application to access Api scopes (**Pre-authorized application** section, select only the **access_as_user** scope)

Last, you must update the **appsettings.json** file with your settings:

```json
    "Domain": "ZZZZZZZZZ.onmicrosoft.com",
    "TenantId": "your tenant ID",
    "ClientId": "your Application ID"
```

for the FrontEnd application add also the Application ID of the Api layer
    
```json
    "TargetApiAppId": "597add50-a83e-4930-8f8f-YYYYYYYY"
```

Browse to [http://localhost:10001](#) and login with an account in your tenant:

![homepage](images/screenshot.png)


