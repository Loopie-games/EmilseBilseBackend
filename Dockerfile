FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY de4aber.emilseBilseBingo.WebAPI/bin/Debug/net6.0/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "de4aber.emilseBilseBingo.WebAPI.dll"]
