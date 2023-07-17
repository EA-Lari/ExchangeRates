#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src/Converter
COPY ["Converter/Converter.Main/Converter.Main.csproj", "Converter.Main/"]
COPY ["Converter/Converter.Core/Converter.Core.csproj", "Converter.Core/"]
WORKDIR /src
COPY ["ExchangeTypes/ExchangeTypes.csproj", "../ExchangeTypes/"]
RUN dotnet restore "Converter/Converter.Main/Converter.Main.csproj"
COPY . .
WORKDIR "/src/Converter/Converter.Main"
RUN dotnet build "Converter.Main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Converter.Main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Converter.Main.dll"]