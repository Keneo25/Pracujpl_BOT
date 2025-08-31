# Pracuj.pl Discord Bot

Bot monitorujący nowe oferty pracy na stronie pracuj.pl i wysyłający powiadomienia na Discord przez webhook.

## Funkcjonalności

- 🔍 Monitorowanie ofert pracy w Olsztynie (IT)
- 📨 Wysyłanie powiadomień na Discord przez webhook
- 🔄 Sprawdzanie co 5 godzin
- ✅ Wykrywanie duplikatów (wysyła tylko nowe oferty)
- 🐳 Gotowy do uruchomienia w Docker/Portainer

## Wymagania

- .NET 8.0 Runtime
- Docker (opcjonalnie)
- Discord Webhook URL

## Konfiguracja

### Zmienne środowiskowe

- `WEBHOOK_URL` - URL webhook Discord (wymagane)
- `CHECK_INTERVAL_HOURS` - Interwał sprawdzania w godzinach (domyślnie: 5)

## Uruchomienie

### Lokalnie

```bash
dotnet run --project Pracujpl_BOT
```

### Docker

```bash
# Zbuduj obraz
docker build -t pracujpl-bot .

# Uruchom kontener
docker run -d \
  --name pracujpl-bot \
  -e WEBHOOK_URL="YOUR_DISCORD_WEBHOOK_URL" \
  -e CHECK_INTERVAL_HOURS=5 \
  --restart unless-stopped \
  pracujpl-bot
```

### Portainer

1. Dodaj to repozytorium jako Git repository w Portainer
2. Stwórz nowy stack z następującym docker-compose.yml:

```yaml
version: '3.8'

services:
  pracujpl-bot:
    build: .
    environment:
      - WEBHOOK_URL=${WEBHOOK_URL}
      - CHECK_INTERVAL_HOURS=${CHECK_INTERVAL_HOURS:-5}
    restart: unless-stopped
    container_name: pracujpl-bot
```

3. Ustaw zmienne środowiskowe w Portainer:
   - `WEBHOOK_URL` - twój Discord webhook URL
   - `CHECK_INTERVAL_HOURS` - opcjonalnie, domyślnie 5

## Jak uzyskać Discord Webhook URL

1. Otwórz Discord i przejdź na serwer
2. Kliknij prawym przyciskiem na kanał, gdzie chcesz otrzymywać powiadomienia
3. Wybierz "Edytuj kanał" → "Integracje" → "Webhooks"
4. Kliknij "Nowy webhook"
5. Skopiuj URL webhook

## Struktura projektu

```
├── Pracujpl_BOT/
│   ├── Models/
│   │   └── JobOffer.cs          # Model oferty pracy
│   ├── Services/
│   │   ├── JobScrapingService.cs # Scraping ofert z pracuj.pl
│   │   └── DiscordService.cs     # Wysyłanie powiadomień Discord
│   └── Program.cs               # Główna logika aplikacji
├── Dockerfile                  # Konfiguracja Docker
└── docker-compose.yml         # Konfiguracja dla Portainer
```

## Troubleshooting

### Bot nie znajduje ofert
- Sprawdź czy strona pracuj.pl nie zmieniła struktury HTML
- Sprawdź logi aplikacji

### Nie wysyła powiadomień na Discord
- Zweryfikuj poprawność webhook URL
- Sprawdź czy webhook ma odpowiednie uprawnienia

### Błędy kompilacji
- Upewnij się, że masz zainstalowany .NET 8.0 SDK
- Uruchom `dotnet restore` w katalogu projektu

## Licencja

MIT License
