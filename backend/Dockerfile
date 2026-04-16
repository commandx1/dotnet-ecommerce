FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/src/Ecommerce.Domain/Ecommerce.Domain.csproj backend/src/Ecommerce.Domain/
COPY backend/src/Ecommerce.Application/Ecommerce.Application.csproj backend/src/Ecommerce.Application/
COPY backend/src/Ecommerce.Infrastructure/Ecommerce.Infrastructure.csproj backend/src/Ecommerce.Infrastructure/
COPY backend/src/Ecommerce.Api/Ecommerce.Api.csproj backend/src/Ecommerce.Api/

RUN dotnet restore backend/src/Ecommerce.Api/Ecommerce.Api.csproj

COPY backend/src backend/src

RUN dotnet publish backend/src/Ecommerce.Api/Ecommerce.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
RUN mkdir -p /app/wwwroot/assets

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]
