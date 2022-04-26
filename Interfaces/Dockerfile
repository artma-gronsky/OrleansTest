FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Interfaces/Interfaces.csproj", "Interfaces/"]
RUN dotnet restore "Interfaces/Interfaces.csproj"
COPY . .
WORKDIR "/src/Interfaces"
RUN dotnet build "Interfaces.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Interfaces.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Interfaces.dll"]
