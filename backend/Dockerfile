# Use the official .NET 6 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj files and restore as distinct layers
COPY src/Fundo.Applications.WebApi/*.csproj src/Fundo.Applications.WebApi/
RUN dotnet restore src/Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish src/Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port
EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"] 