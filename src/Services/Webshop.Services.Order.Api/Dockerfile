FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY src .

RUN dotnet restore Services/Webshop.Services.Order.Api/Webshop.Services.Order.Api.csproj

RUN dotnet publish Services/Webshop.Services.Order.Api/Webshop.Services.Order.Api.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Webshop.Services.Order.Api.dll"]
