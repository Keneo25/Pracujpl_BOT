using Pracujpl_BOT.Services;

namespace Pracujpl_BOT;

class Program
{
    private static readonly string? WebhookUrl = Environment.GetEnvironmentVariable("WEBHOOK_URL");
    
    private static readonly string PracujUrl = "https://it.pracuj.pl/praca/olsztyn;wp?rd=30&cc=5015%2C5016&et=1%2C3%2C17%2C4&wm=hybrid%2Cfull-office%2Chome-office%2Cmobile&iwhpl=false";
    
    private static readonly int CheckIntervalHours = int.TryParse(Environment.GetEnvironmentVariable("CHECK_INTERVAL_HOURS"), out int hours) ? hours : 5;
    
    private static readonly HashSet<string> ProcessedJobIds = new();
    private static readonly HttpClient HttpClient = new();
    
    private static JobScrapingService? _scrapingService;
    private static DiscordService? _discordService;

    static async Task Main()
    {
        Console.WriteLine("🤖 Bot Discord Webhook - Monitor ofert pracy z pracuj.pl");
        Console.WriteLine("=======================================================");
        Console.WriteLine($"📍 Monitorowane miasto: Olsztyn");
        Console.WriteLine($"⏰ Interwał sprawdzania: {CheckIntervalHours}h");
        Console.WriteLine($"🔗 Webhook URL: {(string.IsNullOrEmpty(WebhookUrl) ? "❌ BRAK" : "✅ SKONFIGUROWANY")}");
        Console.WriteLine("=======================================================\n");
        
        if (string.IsNullOrEmpty(WebhookUrl))
        {
            Console.WriteLine("❌ BŁĄD: Nie ustawiono zmiennej środowiskowej WEBHOOK_URL!");
            Console.WriteLine("Ustaw zmienną WEBHOOK_URL z adresem Discord webhook.");
            Environment.Exit(1);
        }
        
        ConfigureServices();
        
        Console.WriteLine("🚀 Rozpoczynam monitorowanie nowych ofert...\n");

        while (true)
        {
            try
            {
                await CheckForNewJobs();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ Sprawdzono oferty. W pamięci: {ProcessedJobIds.Count} ofert.");
                
                await Task.Delay(TimeSpan.FromHours(CheckIntervalHours));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Błąd: {ex.Message}");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }

    private static void ConfigureServices()
    {
        HttpClient.DefaultRequestHeaders.Add("User-Agent", 
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        
        _scrapingService = new JobScrapingService(PracujUrl, HttpClient);
        _discordService = new DiscordService(WebhookUrl!, HttpClient);
    }

    private static async Task CheckForNewJobs()
    {
        if (_scrapingService == null || _discordService == null) return;
        
        try
        {
            var allJobs = await _scrapingService.GetJobOffers();
            var newJobs = allJobs.Where(job => !ProcessedJobIds.Contains(job.Id)).ToList();

            foreach (var job in newJobs)
            {
                ProcessedJobIds.Add(job.Id);
            }

            if (newJobs.Count > 0)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🆕 Znaleziono {newJobs.Count} nowych ofert!");
                
                foreach (var job in newJobs)
                {
                    await _discordService.SendJobNotification(job);
                    await Task.Delay(1000);
                }
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 📭 Brak nowych ofert. Sprawdzonych: {allJobs.Count}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Błąd podczas sprawdzania ofert: {ex.Message}");
        }
    }
}