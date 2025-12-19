# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/BookLendingService.Api/BookLendingService.Api.csproj src/BookLendingService.Api/
COPY src/BookLendingService.Application/BookLendingService.Application.csproj src/BookLendingService.Application/
COPY src/BookLendingService.Domain/BookLendingService.Domain.csproj src/BookLendingService.Domain/
COPY src/BookLendingService.Infrastructure/BookLendingService.Infrastructure.csproj src/BookLendingService.Infrastructure/

RUN dotnet restore src/BookLendingService.Api/BookLendingService.Api.csproj

COPY src/ src/
RUN dotnet publish src/BookLendingService.Api/BookLendingService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

RUN mkdir -p /app/data
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BookLendingService.Api.dll"]
