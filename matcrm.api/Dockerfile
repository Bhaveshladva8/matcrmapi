FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app  
EXPOSE 80  
EXPOSE 443  

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build  
WORKDIR /src  
COPY ["matcrm.api.csproj", "matcrm.api/"]  
# COPY ["matcrm.authentication.jwt.csproj", "matcrm.authentication.jwt/"]  
# COPY ["matcrm.service.csproj", "matcrm.service/"]  
# COPY ["matcrm.data.csproj", "matcrm.data/"]  
RUN dotnet restore "matcrm.api.csproj"  
COPY . .  
WORKDIR "/src/matcrm.api"  
RUN dotnet build "matcrm.api.csproj" -c Release -o /app/build 

FROM build AS publish  
RUN dotnet publish "matcrm.api.csproj" -c Release -o /app/publish 

FROM base AS final  
WORKDIR /app  
COPY --from=publish /app/publish .  
ENTRYPOINT ["dotnet", "matcrm.api.dll"]

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
# FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build-env
# WORKDIR /app

# # Copy csproj and restore as distinct layers
# COPY *.csproj ./
# RUN dotnet restore

# # Copy everything else and build
# COPY . ./
# RUN dotnet publish -c Release -o out

# # Build runtime image
# # FROM mcr.microsoft.com/dotnet/aspnet:6.0
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "aspnet-core-dotnet-core.dll"]
