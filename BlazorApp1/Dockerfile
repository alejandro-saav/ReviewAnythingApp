#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["BlazorApp1/BlazorApp1.csproj", "BlazorApp1/"]
#RUN dotnet restore "BlazorApp1/BlazorApp1.csproj"
#COPY . .
#WORKDIR "/src/BlazorApp1"
#RUN dotnet build "./BlazorApp1.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./BlazorApp1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "BlazorApp1.dll"]

# Stage 1: Base image (for the final runtime application)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80 
# Expose internal port 80


# Stage 2: Build stage (uses SDK, also serves as the base for the develop stage)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy project file(s) for restore (optimizes Docker caching)
COPY ["BlazorApp1/BlazorApp1.csproj", "BlazorApp1/"]
# If you have shared projects, copy them here too before restore
# COPY ["YourSolution.sln", "."]
# COPY ["SharedProject/SharedProject.csproj", "SharedProject/"]
RUN dotnet restore "BlazorApp1/BlazorApp1.csproj"

# Copy all source code into the container (needed by dotnet watch)
COPY . .


# Stage 3: Development Stage (for hot reload)
FROM build AS develop
WORKDIR "/src/BlazorApp1" 
# Set working directory to your project folder

# Ensure the development environment is set: This is crucial for ASP.NET Core
# to enable development-specific features like Hot Reload (often tied to env)
# and to load appsettings.Development.json.
ENV ASPNETCORE_ENVIRONMENT=Development

# Optional: Improves file watching reliability on some systems (e.g., Windows hosts)
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

# The ENTRYPOINT for development: run dotnet watch
# --urls http://+:80 ensures the app listens on port 80 within the container
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://+:80"]


# Stage 4: Publish stage (for production build)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/BlazorApp1" 
# Go back to the project directory before publishing
RUN dotnet publish "./BlazorApp1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Stage 5: Final production image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorApp1.dll"]
