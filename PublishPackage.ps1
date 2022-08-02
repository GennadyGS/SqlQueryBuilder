param (
    [Parameter(Mandatory=$true)]$apiKey,
    $configuration = "Release",
    $packageSource = "https://api.nuget.org/v3/index.json"
)
dotnet pack -c $configuration
dotnet nuget push **/*.nupkg --api-key $apiKey --source $packageSource --skip-duplicate
$version =
    Select-String -Path Directory.Build.props -Pattern "<Version>(\d+\.\d+.\d+)</Version>" `
        | % { $_.Matches } `
        | % { $_.Groups[1].Value }
git tag $version
git push --tags
