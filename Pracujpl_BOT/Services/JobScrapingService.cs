using HtmlAgilityPack;
using Pracujpl_BOT.Models;

namespace Pracujpl_BOT.Services;

public class JobScrapingService
{
    private readonly string _url;
    private readonly HttpClient _httpClient;

    public JobScrapingService(string url, HttpClient httpClient)
    {
        _url = url;
        _httpClient = httpClient;
    }

    public async Task<List<JobOffer>> GetJobOffers()
    {
        var jobs = new List<JobOffer>();
        
        try
        {
            var response = await _httpClient.GetStringAsync(_url);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var jobElements = doc.DocumentNode.SelectNodes("//div[@data-test='default-offer']");
            
            if (jobElements == null)
            {
                Console.WriteLine("Nie znaleziono ofert pracy na stronie.");
                return jobs;
            }

            foreach (var jobElement in jobElements)
            {
                var jobOffer = ParseJobOffer(jobElement);
                if (jobOffer != null)
                {
                    jobs.Add(jobOffer);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas pobierania ofert: {ex.Message}");
        }

        return jobs;
    }

    private JobOffer? ParseJobOffer(HtmlNode jobElement)
    {
        try
        {
            var jobId = ExtractJobId(jobElement);
            if (string.IsNullOrEmpty(jobId)) return null;

            var titleElement = jobElement.SelectSingleNode(".//h2[@data-test='offer-title']//a");
            var title = titleElement?.InnerText?.Trim() ?? "Brak tytułu";
            var link = titleElement?.GetAttributeValue("href", "") ?? "";
            
            if (!string.IsNullOrEmpty(link) && !link.StartsWith("http"))
            {
                link = "https://it.pracuj.pl" + link;
            }

            var companyElement = jobElement.SelectSingleNode(".//h4[@data-test='text-company-name']");
            var company = companyElement?.InnerText?.Trim() ?? "Nieznana firma";

            var locationElement = jobElement.SelectSingleNode(".//h5[@data-test='text-region']");
            var location = locationElement?.InnerText?.Trim() ?? "Nieznana lokalizacja";

            var salaryElement = jobElement.SelectSingleNode(".//span[@data-test='offer-salary']");
            var salary = salaryElement?.InnerText?.Trim() ?? "Brak informacji o wynagrodzeniu";

            return new JobOffer
            {
                Id = jobId,
                Title = title,
                Company = company,
                Location = location,
                Salary = salary,
                Link = link
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas parsowania oferty: {ex.Message}");
            return null;
        }
    }

    private string ExtractJobId(HtmlNode jobElement)
    {
        var linkElement = jobElement.SelectSingleNode(".//h2[@data-test='offer-title']//a");
        var href = linkElement?.GetAttributeValue("href", "");
        
        if (!string.IsNullOrEmpty(href))
        {
            var parts = href.Split('/');
            return parts.LastOrDefault() ?? Guid.NewGuid().ToString();
        }

        var title = jobElement.SelectSingleNode(".//h2[@data-test='offer-title']")?.InnerText?.Trim();
        var company = jobElement.SelectSingleNode(".//h4[@data-test='text-company-name']")?.InnerText?.Trim();
        
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
        {
            return $"{company}-{title}".GetHashCode().ToString();
        }

        return Guid.NewGuid().ToString();
    }
}
