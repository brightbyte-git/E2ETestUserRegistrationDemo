# Use a base image with the .NET 8.0 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["BackendAPI/BackendAPI.csproj", "BackendAPI/"]
RUN dotnet restore "BackendAPI/BackendAPI.csproj"

# Copy the rest of the application code
COPY BackendAPI/ BackendAPI/
WORKDIR "/src/BackendAPI"
RUN dotnet build "BackendAPI.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "BackendAPI.csproj" -c Release -o /app/publish

# Use the runtime image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendAPI.dll"]