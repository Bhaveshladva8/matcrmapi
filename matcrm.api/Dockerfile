FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5100

ENV ASPNETCORE_URLS=http://+:5100

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["api-crm/matcrm.api/matcrm.api.csproj", "api-crm/matcrm.api/"]
RUN dotnet restore "api-crm\matcrm.api\matcrm.api.csproj"
COPY . .
WORKDIR "/src/api-crm/matcrm.api"
RUN dotnet build "matcrm.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "matcrm.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "matcrm.api.dll"]
