FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0 AS build
WORKDIR /src

COPY ["Magnus.API.csproj", "./"]
RUN dotnet restore "Magnus.API.csproj"

COPY . .
RUN dotnet publish "Magnus.API.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Magnus.API.dll"]
