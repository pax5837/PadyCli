dotnet build
dotnet pack
dotnet tool uninstall -g  PadyCli
dotnet tool install --global --ignore-failed-sources --add-source ./nupkg PadyCli