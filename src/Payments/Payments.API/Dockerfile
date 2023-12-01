# Use the Microsoft's official .NET Core image.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5057

# Use SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /build

# Copy the solution-level files
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy csproj and restore as distinct layers
COPY ["src/Payments/Payments.API/Payments.API.csproj", "src/Payments/Payments.API/"]
RUN dotnet restore "src/Payments/Payments.API/Payments.API.csproj"

# Copy the project files and build
COPY ["src/Payments/Payments.API/", "src/Payments/Payments.API/"]
WORKDIR "/build/src/Payments/Payments.API"
# Set the environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_WATCH_SUPPRESS_PROMPTS=1

# Start the application using dotnet watch for hot reloading
ENTRYPOINT ["dotnet", "watch", "run", "--project", "Payments.API.csproj"]
