## See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
## Logging Service Docker
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LoggingService.csproj", "LoggingService/"]
RUN dotnet restore "LoggingService/LoggingService.csproj"

COPY . ./LoggingService
WORKDIR "/src/LoggingService"
RUN dotnet build "LoggingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoggingService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoggingService.dll"]