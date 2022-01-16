param (
    [Parameter(Mandatory=$true)]$apiKey,
    $configuration = "Release",
    $packageSource = "https://api.nuget.org/v3/index.json"
)
dotnet pack -c $configuration
dotnet nuget push **\*.nupkg --api-key $apiKey --source $packageSource --skip-duplicate