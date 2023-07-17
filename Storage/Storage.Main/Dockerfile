#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src/Storage
COPY ["Storage/Storage.Main/Storage.Main.csproj", "Storage.Main/"]
COPY ["Storage/Storage.Database/Storage.Database.csproj", "Storage.Database/"]
COPY ["Storage/Storage.Core/Storage.Core.csproj", "Storage.Core/"]
WORKDIR /src
COPY ["ExchangeTypes/ExchangeTypes.csproj", "ExchangeTypes/"]
RUN dotnet restore "Storage/Storage.Main/Storage.Main.csproj"
COPY . .
WORKDIR "/src/Storage/Storage.Main"
RUN dotnet build "Storage.Main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Storage.Main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Storage.Main.dll"]