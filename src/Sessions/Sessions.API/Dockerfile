# Use the Microsoft's official .NET Core image.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /build

RUN apt-get update && \
    apt-get install -y openjdk-11-jdk && \
    rm -rf /var/lib/apt/lists/* \
    && apt-get clean 

# Set JAVA_HOME environment variable
ENV JAVA_HOME=/usr/lib/jvm/java-11-openjdk-amd64
ENV PATH="${JAVA_HOME}/bin:${PATH}"

# Copy the solution-level files
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy csproj and restore as distinct layers
COPY ["src/Sessions/Sessions.API/Sessions.API.csproj", "src/Sessions/Sessions.API/"]
RUN dotnet restore "src/Sessions/Sessions.API/Sessions.API.csproj"

# Copy the project files and build
COPY ["src/Sessions/Sessions.API/", "src/Sessions/Sessions.API/"]
WORKDIR "/build/src/Sessions/Sessions.API"
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_WATCH_SUPPRESS_PROMPTS=1

ENTRYPOINT ["dotnet", "watch", "run", "--project", "Sessions.API.csproj"]
