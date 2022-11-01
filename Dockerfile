FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY ./moonbaboon.bingo.WebApi/bin/Debug/net6.0 App/
WORKDIR /App
ENTRYPOINT ["dotnet", "moonbaboon.bingo.WebApi.dll"]