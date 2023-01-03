# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
# WORKDIR /app  
# EXPOSE 80  
# EXPOSE 443  

# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build  
# WORKDIR /src  
# COPY ["matcrm.api/matcrm.api.csproj", "matcrm.api/"]  
# COPY ["matcrm.authentication.jwt/matcrm.authentication.jwt.csproj", "matcrm.authentication.jwt/"]  
# COPY ["matcrm.service/matcrm.service.csproj", "matcrm.service/"]  
# COPY ["matcrm.data/matcrm.data.csproj", "matcrm.data/"]  
# RUN dotnet restore "matcrm.api/matcrm.api.csproj"  
# COPY . .  
# WORKDIR "/src/matcrm.api"  
# RUN dotnet build "matcrm.api.csproj" -c Release -o /app/build 

# FROM build AS publish  
# RUN dotnet publish "matcrm.api.csproj" -c Release -o /app/publish 

# FROM base AS final  
# WORKDIR /app  
# COPY --from=publish /app/publish .  
# ENTRYPOINT ["dotnet", "matcrm.api.dll"]  
FROM mcr.microsoft.com/dotnet/core/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "matcrm.api.dll"]
