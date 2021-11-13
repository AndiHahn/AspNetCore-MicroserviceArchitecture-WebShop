FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY src .

RUN dotnet restore WebApps/Webshop.WebApps.ServiceStatus/Webshop.WebApps.ServiceStatus.csproj

RUN dotnet publish WebApps/Webshop.WebApps.ServiceStatus/Webshop.WebApps.ServiceStatus.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Webshop.WebApps.ServiceStatus.dll"]
