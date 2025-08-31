using Pracujpl_BOT.Services;

namespace Pracujpl_BOT;

class Program
{
    private static readonly HashSet<string> ProcessedJobIds = new();
    private static readonly HttpClient HttpClient = new();
    private static JobScrapingService? _scrapingService;
    private static DiscordService? _discordService;

    static async Task Main()
    {
        LoadEnvironmentVariables();
        var webhookUrl = Environment.GetEnvironmentVariable("WEBHOOK_URL");
        var checkIntervalHours = int.TryParse(Environment.GetEnvironmentVariable("CHECK_INTERVAL_HOURS"), out int hours) ? hours : 5;
        var pracujUrl = "https://it.pracuj.pl/praca/olsztyn;wp?rd=30&cc=5015%2C5016&et=1%2C3%2C17%2C4&wm=hybrid%2Cfull-office%2Chome-office%2Cmobile&iwhpl=false";
        Console.WriteLine("Bot Discord Webhook - Monitor ofert pracy z pracuj.pl");
        Console.WriteLine("=======================================================");
        Console.WriteLine($"Monitorowane miasto: Olsztyn");
        Console.WriteLine($"Interwał sprawdzania: {checkIntervalHours}h");
        Console.WriteLine($"Webhook URL: {(string.IsNullOrEmpty(webhookUrl) ? "BRAK" : "SKONFIGUROWANY")}");
        Console.WriteLine("=======================================================\n");
        if (string.IsNullOrEmpty(webhookUrl))
        {
            Console.WriteLine("❌ BŁĄD: Nie ustawiono zmiennej środowiskowej WEBHOOK_URL!");
            Console.WriteLine("Ustaw zmienną WEBHOOK_URL lub dodaj plik .env w katalogu głównym rozwiązania.");
            Console.WriteLine("Przykład (.env): WEBHOOK_URL=https://discord.com/api/webhooks/ID/TOKEN");
            Environment.Exit(1);
        }
        ConfigureServices(webhookUrl, pracujUrl);
        Console.WriteLine("Rozpoczynam monitorowanie nowych ofert...\n");
        while (true)
        {
            try
            {
                await CheckForNewJobs();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Sprawdzono oferty. W pamięci: {ProcessedJobIds.Count} ofert.");
                await Task.Delay(TimeSpan.FromHours(checkIntervalHours));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Błąd: {ex.Message}");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }

    private static void LoadEnvironmentVariables()
    {
        var tried = new List<string>();
        var startDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        DirectoryInfo? dir = startDir;
        string? foundPath = null;
        for (int i = 0; i < 8 && dir != null; i++)
        {
            var candidate = Path.Combine(dir.FullName, ".env");
            tried.Add(candidate);
            if (File.Exists(candidate))
            {
                foundPath = candidate;
                break;
            }
            dir = dir.Parent;
        }
        if (foundPath == null)
        {
            Console.WriteLine("ℹ️ Plik .env nie znaleziony. Sprawdzone ścieżki:");
            foreach (var t in tried) Console.WriteLine(" - " + t);
            return;
        }
        Console.WriteLine("📁 Wczytano plik .env: " + foundPath);
        foreach (var line in File.ReadAllLines(foundPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#')) continue;
            var parts = line.Split('=', 2);
            if (parts.Length != 2) continue;
            var key = parts[0].Trim();
            var value = parts[1].Trim().Trim('\"');
            Environment.SetEnvironmentVariable(key, value);
            Console.WriteLine($"✅ Ustawiono {key}");
        }
    }

    private static void ConfigureServices(string webhookUrl, string pracujUrl)
    {
        if (!HttpClient.DefaultRequestHeaders.Contains("User-Agent"))
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        _scrapingService = new JobScrapingService(pracujUrl, HttpClient);
        _discordService = new DiscordService(webhookUrl, HttpClient);
    }

    private static async Task CheckForNewJobs()
    {
        if (_scrapingService == null || _discordService == null) return;
        try
        {
            var allJobs = await _scrapingService.GetJobOffers();
            var newJobs = allJobs.Where(job => !ProcessedJobIds.Contains(job.Id)).ToList();
            foreach (var job in newJobs) ProcessedJobIds.Add(job.Id);
            if (newJobs.Count > 0)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Znaleziono {newJobs.Count} nowych ofert!");
                foreach (var job in newJobs)
                {
                    await _discordService.SendJobNotification(job);
                    await Task.Delay(1000);
                }
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Brak nowych ofert. Sprawdzonych: {allJobs.Count}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas sprawdzania ofert: {ex.Message}");
        }
    }
}