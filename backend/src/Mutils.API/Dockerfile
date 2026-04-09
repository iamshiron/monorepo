FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/src/Mutils.Api/Shiron.Mutils.Api.csproj backend/src/Mutils.Api/
COPY backend/src/Mutils.Core/Shiron.Mutils.Core.csproj backend/src/Mutils.Core/
COPY backend/src/Mutils.Infrastructure/Shiron.Mutils.Infrastructure.csproj backend/src/Mutils.Infrastructure/

RUN dotnet restore backend/src/Mutils.Api/Shiron.Mutils.Api.csproj

COPY backend/src/ backend/src/

RUN dotnet publish backend/src/Mutils.Api/Shiron.Mutils.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Shiron.Mutils.Api.dll"]
