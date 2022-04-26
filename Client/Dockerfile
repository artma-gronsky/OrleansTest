FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cleint/Cleint.csproj", "Cleint/"]
RUN dotnet restore "Cleint/Cleint.csproj"
COPY . .
WORKDIR "/src/Cleint"
RUN dotnet build "Cleint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cleint.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cleint.dll"]
