param (
    [Parameter(Mandatory=$true)]$apiKey,
    $configuration = "Release",
    $packageSource = "https://api.nuget.org/v3/index.json"
)

# Enable strict error handling
$ErrorActionPreference = "Stop"

function Test-CommandSuccess {
    param(
        [Parameter(Mandatory=$true)]
        [string]$CommandDescription
    )

    if ($LastExitCode -ne 0) {
        Write-Error "$CommandDescription failed with exit code $LastExitCode"
        exit $LastExitCode
    }
}

Write-Host "Building package..."
dotnet pack -c $configuration
Test-CommandSuccess "dotnet pack"

Write-Host "Pushing package to NuGet..."
dotnet nuget push **/*.nupkg --api-key $apiKey --source $packageSource --skip-duplicate
Test-CommandSuccess "dotnet nuget push"

Write-Host "Creating git tag..."
$version =
    Select-String -Path Directory.Build.props -Pattern "<Version>(\d+\.\d+.\d+)</Version>" `
        | % { $_.Matches } `
        | % { $_.Groups[1].Value }

if (-not $version) {
    Write-Error "Could not extract version from Directory.Build.props"
    exit 1
}

git tag $version
Test-CommandSuccess "git tag"

git push --tags
Test-CommandSuccess "git push --tags"

Write-Host "Package published successfully with version $version"
