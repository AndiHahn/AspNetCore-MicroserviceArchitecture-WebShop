FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY src .

RUN dotnet restore ApiGateways/Webshop.ApiGateways.Bff.Aggregator/Webshop.ApiGateways.Bff.Aggregator.csproj

RUN dotnet publish ApiGateways/Webshop.ApiGateways.Bff.Aggregator/Webshop.ApiGateways.Bff.Aggregator.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Webshop.ApiGateways.Bff.Aggregator.dll"]
