try {
#    Write-Host("Generating third party license info")
#    thirdlicense --project PadyCli.ConsoleApp.csproj
#    if ($LASTEXITCODE -ne 0) { throw "thirdlicense command failed" }

    Write-Host("Building")
    dotnet build
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

    Write-Host("Packing")
    dotnet pack --output ./nupkg
    if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed" }

    Write-Host("Uninstall previous")
    dotnet tool uninstall -g PadyCli.ConsoleApp
    # Don't check exit code here as it might not be installed

    Write-Host("Install")
    dotnet tool install --global --ignore-failed-sources --add-source ./nupkg PadyCli.ConsoleApp
    if ($LASTEXITCODE -ne 0) { throw "dotnet tool install failed" }

    Write-Host("All commands completed successfully!")
}
catch {
    Write-Error "\n\n\nInstallation script failed: $_"
    exit 1
}