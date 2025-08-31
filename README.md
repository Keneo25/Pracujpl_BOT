# Pracuj.pl Discord Bot

Bot monitorujący nowe oferty pracy na stronie pracuj.pl i wysyłający powiadomienia na Discord przez webhook.

## Funkcjonalności

- 🔍 Monitorowanie ofert pracy w Olsztynie (IT)
- 📨 Wysyłanie powiadomień na Discord przez webhook
- 🔄 Sprawdzanie co 5 godzin
- ✅ Wykrywanie duplikatów (wysyła tylko nowe oferty)
- 🐳 Gotowy do uruchomienia w Docker/Portainer

## Konfiguracja

### Zmienne środowiskowe

Skopiuj plik `.env.example` do `.env` i ustaw:

- `WEBHOOK_URL` - URL webhook Discord (wymagane)
- `CHECK_INTERVAL_HOURS` - Interwał sprawdzania w godzinach (domyślnie: 5)

## Uruchomienie

### Lokalnie

```bash
# Skopiuj przykładowy plik konfiguracji
cp .env.example .env

# Edytuj .env i ustaw swój WEBHOOK_URL
# Następnie uruchom:
dotnet run --project Pracujpl_BOT
```

### Docker Compose

```bash
# Skopiuj przykładowy plik konfiguracji
cp .env.example .env

# Edytuj .env i ustaw swój WEBHOOK_URL
# Następnie uruchom:
docker-compose up -d
```

### Portainer - Instrukcja krok po kroku

#### 1. Przygotowanie repozytorium GitHub

1. Stwórz nowe repozytorium na GitHub
2. Sklonuj to repozytorium lokalnie
3. Skopiuj wszystkie pliki z bota do repozytorium
4. **WAŻNE**: Nie dodawaj pliku `.env` do repozytorium (jest w .gitignore)
5. Wyślij kod na GitHub:
   ```bash
   git add .
   git commit -m "Initial commit - Pracuj.pl Discord Bot"
   git push origin main
   ```

#### 2. Konfiguracja w Portainer

1. **Zaloguj się do Portainer**
2. **Przejdź do "Stacks"**
3. **Kliknij "Add stack"**
4. **Ustaw nazwę**: `pracujpl-bot`
5. **Wybierz "Repository"**
6. **Ustaw Repository URL**: `https://github.com/TWOJ_USERNAME/NAZWA_REPO.git`
7. **Ustaw Compose path**: `docker-compose.yml`

#### 3. Ustawienie zmiennych środowiskowych

W sekcji **"Environment variables"** dodaj:

| Nazwa | Wartość |
|-------|---------|
| `WEBHOOK_URL` | `https://discord.com/api/webhooks/1411344314734477322/a_p1wg4e34GJ5ooDCjx8mw04nd7zQDi670uq336CSNYYiZavOuPAEnmcQ6ITQZsp4Cv3jak` |
| `CHECK_INTERVAL_HOURS` | `5` |

#### 4. Wdrożenie

1. **Kliknij "Deploy the stack"**
2. **Poczekaj na zbudowanie obrazu** (może potrwać kilka minut)
3. **Sprawdź logi** w sekcji "Containers"

#### 5. Monitorowanie

- **Logi**: Containers → pracujpl-bot → Logs
- **Restart**: Containers → pracujpl-bot → Restart
- **Status**: Containers → pracujpl-bot

## Jak uzyskać Discord Webhook URL

1. Otwórz Discord i przejdź na serwer
2. Kliknij prawym przyciskiem na kanał, gdzie chcesz otrzymywać powiadomienia
3. Wybierz "Edytuj kanał" → "Integracje" → "Webhooks"
4. Kliknij "Nowy webhook"
5. Skopiuj URL webhook

## Troubleshooting

### Problem z budowaniem w Portainer
- Sprawdź czy repozytorium jest publiczne
- Upewnij się, że ścieżka do docker-compose.yml jest poprawna

### Bot nie znajduje ofert
- Sprawdź logi kontejnera w Portainer
- Zweryfikuj czy strona pracuj.pl nie zmieniła struktury

### Nie wysyła powiadomień na Discord
- Zweryfikuj poprawność webhook URL
- Sprawdź czy webhook ma odpowiednie uprawnienia

### Aktualizacja bota
1. Wyślij zmiany na GitHub
2. W Portainer: Stacks → pracujpl-bot → "Update the stack"
3. Kliknij "Pull and redeploy"

## Struktura projektu

```
├── Pracujpl_BOT/
│   ├── Models/
│   │   └── JobOffer.cs
│   ├── Services/
│   │   ├── JobScrapingService.cs
│   │   └── DiscordService.cs
│   └── Program.cs
├── .env.example                # Przykład konfiguracji
├── .env                       # Twoja konfiguracja (nie commituj!)
├── docker-compose.yml         # Konfiguracja dla Portainer
├── Dockerfile                 # Obraz Docker
└── README.md
```
