#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src/Crawler
COPY ["Crawler/Crawler.Main/Crawler.Main.csproj", "Crawler.Main/"]
COPY ["Crawler/Crawler.Core/Crawler.Core.csproj", "Crawler.Core/"]
WORKDIR /src
COPY ["ExchangeTypes/ExchangeTypes.csproj", "ExchangeTypes/"]
RUN dotnet restore "Crawler/Crawler.Main/Crawler.Main.csproj"
COPY . .
WORKDIR "/src/Crawler/Crawler.Main"
RUN dotnet build "Crawler.Main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Crawler.Main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crawler.Main.dll"]