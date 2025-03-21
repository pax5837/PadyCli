Write-Host("Generating third party license info")
thirdlicense --project PadyCli.ConsoleApp.csproj

Write-Host("Building")
dotnet build

Write-Host("Packing")
dotnet pack --output ./nupkg

Write-Host("Uninstall previous")
dotnet tool uninstall -g PadyCli.ConsoleApp

Write-Host("Install")
dotnet tool install --global --ignore-failed-sources --add-source ./nupkg PadyCli.ConsoleApp