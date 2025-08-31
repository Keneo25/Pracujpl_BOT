using System.Text;
using Newtonsoft.Json;
using Pracujpl_BOT.Models;

namespace Pracujpl_BOT.Services;

public class DiscordService
{
    private readonly string _webhookUrl;
    private readonly HttpClient _httpClient;

    public DiscordService(string webhookUrl, HttpClient httpClient)
    {
        _webhookUrl = webhookUrl;
        _httpClient = httpClient;
    }

    public async Task SendJobNotification(JobOffer job)
    {
        try
        {
            var embed = new
            {
                title = "🆕 Nowa oferta pracy!",
                color = 0x00ff00,
                fields = new[]
                {
                    new { name = "💼 Stanowisko", value = job.Title, inline = false },
                    new { name = "🏢 Firma", value = job.Company, inline = true },
                    new { name = "📍 Lokalizacja", value = job.Location, inline = true },
                    new { name = "💰 Wynagrodzenie", value = job.Salary, inline = false },
                    new { name = "🔗 Link", value = $"[Zobacz ofertę]({job.Link})", inline = false }
                },
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                footer = new
                {
                    text = "Pracuj.pl Bot",
                    icon_url = "https://cdn.discordapp.com/emojis/1234567890123456789.png"
                }
            };

            var payload = new
            {
                username = "Pracuj.pl Bot",
                avatar_url = "https://pracuj.pl/favicon.ico",
                embeds = new[] { embed }
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(_webhookUrl, content);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ Wysłano powiadomienie: {job.Title} - {job.Company}");
            }
            else
            {
                Console.WriteLine($"❌ Błąd wysyłania powiadomienia: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas wysyłania powiadomienia Discord: {ex.Message}");
        }
    }
}
