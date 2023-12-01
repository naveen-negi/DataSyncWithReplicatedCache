# Use the Microsoft's official .NET Core image.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5260

# Use SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /build

# Copy the solution-level files
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy csproj and restore as distinct layers
COPY ["src/ProductPricing/ProductPricing.API/ProductPricing.API.csproj", "src/ProductPricing/ProductPricing.API/"]
RUN ls -la
RUN dotnet restore "src/ProductPricing/ProductPricing.API/ProductPricing.API.csproj"

# Copy the project files and build
COPY ["src/ProductPricing/ProductPricing.API/", "src/ProductPricing/ProductPricing.API/"]
WORKDIR "/build/src/ProductPricing/ProductPricing.API"

ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_WATCH_SUPPRESS_PROMPTS=1
ENV Logging__LogLevel__Default=Trace
ENTRYPOINT ["dotnet", "watch", "run", "--project", "ProductPricing.API.csproj"]
