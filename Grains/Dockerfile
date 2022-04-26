FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Grains/Grains.csproj", "Grains/"]
RUN dotnet restore "Grains/Grains.csproj"
COPY . .
WORKDIR "/src/Grains"
RUN dotnet build "Grains.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Grains.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Grains.dll"]
