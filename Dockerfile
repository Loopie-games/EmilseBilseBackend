FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY ./EmilseBilseBackend/EmilseBilseBingo/bin/Debug/net6.0 App/
WORKDIR /App
ENTRYPOINT ["dotnet", "de4aber.emilseBilseBingo.WebAPI.dll"]
