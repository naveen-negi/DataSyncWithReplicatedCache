# Use the Microsoft's official .NET Core image.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5118

# Use SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /build

# Copy the solution-level files
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy csproj and restore as distinct layers
COPY ["src/SessionOrchestrator/SessionOrchestrator.csproj", "src/SessionOrchestrator/"]
RUN dotnet restore "src/SessionOrchestrator/SessionOrchestrator.csproj"

# Copy the project files and build
COPY ["src/SessionOrchestrator/", "src/SessionOrchestrator/"]
WORKDIR "/build/src/SessionOrchestrator"
# Set the environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_WATCH_SUPPRESS_PROMPTS=1

# Start the application using dotnet watch for hot reloading
ENTRYPOINT ["dotnet", "watch", "run", "--project", "SessionOrchestrator.csproj"]
