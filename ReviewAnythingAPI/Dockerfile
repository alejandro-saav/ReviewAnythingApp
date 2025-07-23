FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReviewAnythingAPI/ReviewAnythingAPI.csproj", "ReviewAnythingAPI/"]
RUN dotnet restore "ReviewAnythingAPI/ReviewAnythingAPI.csproj"
COPY . .
WORKDIR "/src/ReviewAnythingAPI"
RUN dotnet build "./ReviewAnythingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReviewAnythingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewAnythingAPI.dll"]
