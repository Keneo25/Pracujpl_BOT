# Użyj obrazu .NET SDK do budowania aplikacji
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Skopiuj pliki projektu
COPY Pracujpl_BOT/Pracujpl_BOT.csproj Pracujpl_BOT/
RUN dotnet restore "Pracujpl_BOT/Pracujpl_BOT.csproj"

# Skopiuj pozostałe pliki i zbuduj aplikację
COPY . .
WORKDIR "/src/Pracujpl_BOT"
RUN dotnet build "Pracujpl_BOT.csproj" -c Release -o /app/build

# Publikuj aplikację
RUN dotnet publish "Pracujpl_BOT.csproj" -c Release -o /app/publish

# Użyj obrazu runtime do uruchomienia aplikacji
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Ustaw zmienne środowiskowe
ENV WEBHOOK_URL=""
ENV CHECK_INTERVAL_HOURS=5

ENTRYPOINT ["dotnet", "Pracujpl_BOT.dll"]
