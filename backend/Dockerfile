# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY MoneyTrace.sln ./
COPY MoneyTrace.Api/MoneyTrace.Api.csproj MoneyTrace.Api/
COPY MoneyTrace.Application/MoneyTrace.Application.csproj MoneyTrace.Application/
COPY MoneyTrace.Domain/MoneyTrace.Domain.csproj MoneyTrace.Domain/
COPY MoneyTrace.Infrastructure/MoneyTrace.Infrastructure.csproj MoneyTrace.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish
WORKDIR /src/MoneyTrace.Api
RUN dotnet publish -c Release -o /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENTRYPOINT ["dotnet", "MoneyTrace.Api.dll"]
