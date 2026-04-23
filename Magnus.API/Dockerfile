# Build stage (.NET 10)
FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0 AS build
WORKDIR /app

# Copy everything (simpler & safer)
COPY . ./

# Restore & publish
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# Runtime stage (.NET 10)
FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Magnus.API.dll"]