FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

#COPY *.csproj .
#COPY src/Services/Webshop.Services.Catalog.Api .

COPY src .

RUN dotnet restore Services/Webshop.Services.Catalog.Api/Webshop.Services.Catalog.Api.csproj

RUN dotnet publish Services/Webshop.Services.Catalog.Api/Webshop.Services.Catalog.Api.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Webshop.Services.Catalog.Api.dll"]
