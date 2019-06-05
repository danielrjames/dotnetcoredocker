FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
# EXPOSE 80 # comment this out because I expose in docker-compose for consistancy

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["core.Web/core.Web.csproj", "core.Web/"]
COPY ["core.Data/core.Data.csproj", "core.Data/"]
COPY ["core.Domain/core.Domain.csproj", "core.Domain/"]
COPY ["core.Services/core.Services.csproj", "core.Services/"]
RUN dotnet restore "core.Web/core.Web.csproj"
COPY . .
WORKDIR /src/core.Web
RUN dotnet build "core.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "core.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "core.Web.dll"]