# Stage 1: Build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
 
# Copy solution and restore dependencies

COPY ["ActivityManagementSystem.API.sln", "."]

COPY ["ActivityManagementSystem.API/ActivityManagementSystem.API.csproj", "ActivityManagementSystem.API/"]

COPY ["ActivityManagementSystem.BLL/ActivityManagementSystem.BLL.csproj", "ActivityManagementSystem.BLL/"]

COPY ["ActivityManagementSystem.CrossCutting/ActivityManagementSystem.CrossCutting.csproj", "ActivityManagementSystem.CrossCutting/"]

COPY ["ActivityManagementSystem.DAL/ActivityManagementSystem.DAL.csproj", "ActivityManagementSystem.DAL/"]

COPY ["ActivityManagementSystem.Domain/ActivityManagementSystem.Domain.csproj", "ActivityManagementSystem.Domain/"]
 
RUN dotnet restore "ActivityManagementSystem.API.sln"
 
# Copy everything else and build

COPY . .

WORKDIR "/src/ActivityManagementSystem.API"

RUN dotnet publish -c Release -o /app/publish
 
# Stage 2: Runtime

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /src

COPY --from=build /app/publish .
 
# Expose port (adjust according to your app)

EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
 
ENTRYPOINT ["dotnet", "ActivityManagementSystem.API.dll"]
 
