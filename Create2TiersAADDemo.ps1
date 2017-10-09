#  getting the right TENANT ID (if multiple subscriptions)
$login = Login-AzureRmAccount
$subs = Get-AzureRmSubscription

#  if multiple directories select the right array index
$tenantId = $subs[1].TenantId 

#  azure ad 
#  when creating the resource from the portal, the portal itself does many steps under the hood
#  in this case we need to manually do them 
#   1- register the apps
#   2- register a service principal for each app
#   3- build and add required parameters to the manifest:
#         - user read and signin for azure active directory
#         - permission to access to api for the web app 
#-----------------------
Connect-AzureAD -TenantId $tenantId

#  Grab the Azure AD Service principal, needed to build the user read access permission
$aad = (Get-AzureADServicePrincipal | `
    where {$_.ServicePrincipalNames.Contains("https://graph.windows.net")})[0]
#  Grab the User.Read permission
$userRead = $aad.Oauth2Permissions | ? {$_.Value -eq "User.Read"}
 
#  Resource Access User.Read + Sign in for Azure AD
$readUserAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
  ResourceAppId=$aad.AppId ;
  ResourceAccess=[Microsoft.Open.AzureAD.Model.ResourceAccess]@{
    Id = $userRead.Id ;
    Type = "Scope"}}
 

#  Create Api App
Write-Host "Creating API app..."
$apiApp = New-AzureADApplication -DisplayName AAD.2tiers.Api -IdentifierUris "http://AAD.2tiers.Api" -Oauth2AllowImplicitFlow $true -Homepage "http://localhost:10000" -ReplyUrls "http://localhost:10000" -RequiredResourceAccess $readUserAccess

# Create a Service Principal for the Api
Write-Host "Creating Service Principal for API app..."
$spApi = New-AzureADServicePrincipal -AppId $apiApp.AppId

#  Grab the user-impersonation permission for Api 
$apiUserImpersonation = $spApi.Oauth2Permissions | ?{$_.Value -eq "user_impersonation"}
  
#  Resource Access description to access the Api app
Write-Host "Building API access permission..."
$apiAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
  ResourceAppId=$apiApp.AppId ;
  ResourceAccess=[Microsoft.Open.AzureAD.Model.ResourceAccess]@{
    Id = $apiUserImpersonation.Id ;
    Type = "Scope"}}

#  Create Web App
Write-Host "Creating Web app..."
$webApp = New-AzureADApplication -DisplayName AAD.2tiers.Web -IdentifierUris "http://AAD.2tiers.Web" -Oauth2AllowImplicitFlow $true -Homepage "http://localhost:10001" -ReplyUrls "http://localhost:10001/signin-oidc" -RequiredResourceAccess $readUserAccess, $apiAccess

Write-Host "Creating Service Principal for Web app..."
$spWebApp = New-AzureADServicePrincipal -AppId $webApp.AppId

Write-Host "Created Web App"
Write-Host "-----------------------"
Write-Host $webApp 

Write-Host "Created Api App"
Write-Host "-----------------------"
Write-Host $apiApp 