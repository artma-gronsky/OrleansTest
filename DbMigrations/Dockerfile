FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DbMigrations/DbMigrations.csproj", "DbMigrations/"]
RUN dotnet restore "DbMigrations/DbMigrations.csproj"
COPY . .
WORKDIR "/src/DbMigrations"
RUN dotnet build "DbMigrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DbMigrations.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DbMigrations.dll"]
