FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SiloHost/SiloHost.csproj", "SiloHost/"]
RUN dotnet restore "SiloHost/SiloHost.csproj"
COPY . .
WORKDIR "/src/SiloHost"
RUN dotnet build "SiloHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SiloHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SiloHost.dll"]
