Write-Host "Do data migration"
dotnet tool update --global dotnet-ef
dotnet ef database update --project App/App.csproj --context App.Data.ApplicationDbContext

Write-Host "Running tests ..."
dotnet test Api.Test
dotnet test BL.Test

dotnet user-secrets init --project Api
$secrets = dotnet user-secrets list --project Api

if ($secrets -contains 'No secrets configured for this application.')
{
    $apiKey = Read-Host -Prompt 'Please provide API Key: ety..'
    dotnet user-secrets set PlateSolverOptions:ApiKey $apiKey --project Api
}