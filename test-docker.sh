#!/bin/bash
# Skrypt do testowania bota lokalnie

echo "🐳 Budowanie obrazu Docker..."
docker build -t pracujpl-bot .

echo "✅ Obraz zbudowany pomyślnie!"

echo "🚀 Uruchamianie bota z testowym webhook..."
echo "Ustaw WEBHOOK_URL w zmiennych środowiskowych lub użyj:"
echo "docker run -e WEBHOOK_URL='YOUR_WEBHOOK_URL' pracujpl-bot"

# Opcjonalnie uruchom z domyślnym webhook (usuń komentarz poniżej)
# docker run -d --name pracujpl-bot-test -e WEBHOOK_URL="YOUR_WEBHOOK_URL" pracujpl-bot
