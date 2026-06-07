using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using ExcelHelperEx;
using Newtonsoft.Json;
using PhantomClientCore;
using UsedCarsScraper.GeneralModels;
using UsedCarsScraper.LaCentraleModels;

namespace UsedCarsScraper.Services;

public class LaCentraleService
{
    private const string SearchBaseUrl = "https://www.lacentrale.fr/listing";
    private const string SiteBaseUrl = "https://www.lacentrale.fr";
    private const int PageSize = 16;

    public async Task Start(
        List<LaCentraleInputModel> inputs,
        IProgress<(int Percentage, string Message)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory("lacentral configs");
        Directory.CreateDirectory("lacentral json outputs");
        Directory.CreateDirectory("lacentral outputs");

        await PhantomTLS.InitializeAsync();
        await using var client = new PhantomClient(new PhantomClientOptions
        {
            ClientIdentifier = "chrome_120",
            Timeout = 30000
        });

        var cars = new List<Car>();
        var validInputs = inputs.Where(x => x.Make != null).ToList();
        for (var i = 0; i < validInputs.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var startPercentage = GetInputProgressPercentage(i, validInputs.Count);
            var endPercentage = GetInputProgressPercentage(i + 1, validInputs.Count);

            cars.AddRange(await StartScraping(
                client,
                validInputs[i],
                startPercentage,
                endPercentage,
                progress,
                cancellationToken));

            await Task.Delay(1000, cancellationToken);
        }

        var json = JsonConvert.SerializeObject(cars, Formatting.Indented);
        await File.WriteAllTextAsync(
            $"lacentral json outputs/lacentral_cars_json_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.json",
            json,
            cancellationToken);
        cars.SaveToExcel($"lacentral outputs/lacentral_cars_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx");
    }

    private async Task<List<Car>> StartScraping(
        PhantomClient client,
        LaCentraleInputModel input,
        int startPercentage,
        int endPercentage,
        IProgress<(int Percentage, string Message)>? progress,
        CancellationToken cancellationToken)
    {
        var cars = new List<Car>();
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var page = 1;
        var carsFound = 0;
        var totalCars = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var url = BuildSearchUrl(input, page);
            progress?.Report((GetPageProgressPercentage(startPercentage, endPercentage, page, cars.Count, Math.Max(carsFound, 1)),
                $"LaCentrale listing URL: {url}"));
            progress?.Report((GetPageProgressPercentage(startPercentage, endPercentage, page, cars.Count, Math.Max(carsFound, 1)),
                $"Loading LaCentrale page {page}: {input.Make?.Name}/{input.Model?.Name}/{input.SubModel?.Name}"));

            var body = await GetString(client, url, cancellationToken);
            var searchPage = ParseSearchPage(body);
            totalCars = Math.Max(totalCars, searchPage.TotalCars ?? searchPage.Cars.Count);
            carsFound += searchPage.Cars.Count;

            if (searchPage.Cars.Count == 0)
            {
                progress?.Report((endPercentage,
                    $"No LaCentrale listings found for {input.Make?.Name}/{input.Model?.Name}/{input.SubModel?.Name}."));
                break;
            }

            foreach (var car in searchPage.Cars)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!seenUrls.Add(car.Url)) continue;

                cars.Add(car);
                var percentage = GetPageProgressPercentage(
                    startPercentage,
                    endPercentage,
                    page,
                    cars.Count,
                    Math.Max(totalCars, carsFound));
                progress?.Report((percentage,
                    $"{cars.Count} of {Math.Max(totalCars, carsFound)} LaCentrale cars scraped: {input.Make?.Name}/{input.Model?.Name}"));
            }

            if (searchPage.Cars.Count < PageSize || cars.Count >= totalCars)
            {
                break;
            }

            page++;
        }

        cars = ApplyPostFilters(cars, input);
        progress?.Report((endPercentage,
            $"{cars.Count} of {Math.Max(totalCars, carsFound)} LaCentrale cars kept after filters."));
        return cars;
    }

    private static async Task<string> GetString(
        PhantomClient client,
        string url,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = await client.GetAsync(url, CreateRequestOptions());
        if (!response.IsSuccess || string.IsNullOrWhiteSpace(response.Body))
        {
            throw new InvalidOperationException(
                $"LaCentrale request failed with status {response.Status}. Response starts with: {Preview(response.Body)}");
        }

        return response.Body;
    }

    private static RequestOptions CreateRequestOptions()
    {
        return new RequestOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8",
                ["Referer"] = "https://www.lacentrale.fr/",
                ["Sec-Fetch-Site"] = "none",
                ["Sec-Fetch-Mode"] = "navigate",
                ["Sec-Fetch-Dest"] = "document",
                ["Upgrade-Insecure-Requests"] = "1",
                ["User-Agent"] =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
                ["Accept-Language"] = "fr-FR,fr;q=0.9,en-US;q=0.8,en;q=0.7"
            }
        };
    }

    private static string BuildSearchUrl(LaCentraleInputModel input, int page)
    {
        var makeModel = input.Model == null
            ? input.Make?.Name
            : $"{input.Make?.Name}::{input.Model.Name}";
        var subModel = input.SubModel?.Name;

        var query = new Dictionary<string, string?>
        {
            ["makesModelsCommercialNames"] = makeModel,
            ["versions"] = subModel,
            ["energies"] = input.FuelType,
            ["priceMin"] = input.PriceFrom,
            ["priceMax"] = input.PriceTo,
            ["mileageMin"] = input.MileAgeFrom,
            ["mileageMax"] = input.MileAgeTo,
            ["powerDINMin"] = input.DinPowerFrom,
            ["powerDINMax"] = input.DinPowerTo,
            ["yearMin"] = input.RegistrationYear?.ToString(CultureInfo.InvariantCulture),
            ["page"] = page.ToString(CultureInfo.InvariantCulture)
        };

        var queryString = string.Join("&", query
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(NormalizeQueryValue(x.Value!))}"));
        Console.WriteLine($"{SearchBaseUrl}?{queryString}");
        return $"{SearchBaseUrl}?{queryString}";
    }

    private static string NormalizeQueryValue(string value)
    {
        return value.Trim();
    }

    private static SearchPage ParseSearchPage(string body)
    {
        if (TryParseSearchJson(body, out var searchPage))
        {
            return searchPage;
        }

        foreach (var json in ExtractEmbeddedJson(body))
        {
            if (TryParseSearchJson(json, out searchPage))
            {
                return searchPage;
            }
        }

        return ParseListingLinks(body);
    }

    private static bool TryParseSearchJson(string json, out SearchPage searchPage)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            searchPage = ParseSearchDocument(document.RootElement);
            return searchPage.Cars.Count > 0 || searchPage.TotalCars != null;
        }
        catch
        {
            searchPage = new SearchPage([], null);
            return false;
        }
    }

    private static SearchPage ParseSearchDocument(JsonElement root)
    {
        var totalCars = FindFirstInt(root, "total", "totalCount", "totalHits", "numFound", "nextTotal", "homeTotal");
        if (!TryGetPropertyRecursive(root, "hits", out var hits) ||
            hits.ValueKind != JsonValueKind.Array)
        {
            return new SearchPage([], totalCars);
        }

        var cars = hits.EnumerateArray()
            .Select(ParseHit)
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Url))
            .Select(x => x!)
            .ToList();

        return new SearchPage(cars, totalCars);
    }

    private static IEnumerable<string> ExtractEmbeddedJson(string html)
    {
        foreach (Match match in Regex.Matches(
                     html,
                     @"<script[^>]*id=[""']__NEXT_DATA__[""'][^>]*>(.*?)</script>",
                     RegexOptions.IgnoreCase | RegexOptions.Singleline))
        {
            var json = WebUtility.HtmlDecode(match.Groups[1].Value).Trim();
            if (!string.IsNullOrWhiteSpace(json)) yield return json;
        }

        foreach (var marker in new[]
                 {
                     "window.__PRELOADED_STATE__",
                     "window.__INITIAL_STATE__",
                     "__PRELOADED_STATE__",
                     "__INITIAL_STATE__"
                 })
        {
            var index = html.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0) continue;

            var objectStart = html.IndexOf('{', index);
            var json = objectStart >= 0 ? ExtractBalancedJsonObject(html, objectStart) : null;
            if (!string.IsNullOrWhiteSpace(json)) yield return WebUtility.HtmlDecode(json);
        }

        foreach (Match match in Regex.Matches(
                     html,
                     @"<script[^>]*type=[""']application/json[""'][^>]*>(.*?)</script>",
                     RegexOptions.IgnoreCase | RegexOptions.Singleline))
        {
            var json = WebUtility.HtmlDecode(match.Groups[1].Value).Trim();
            if (!string.IsNullOrWhiteSpace(json)) yield return json;
        }
    }

    private static string? ExtractBalancedJsonObject(string text, int startIndex)
    {
        var depth = 0;
        var inString = false;
        var escaped = false;

        for (var i = startIndex; i < text.Length; i++)
        {
            var c = text[i];
            if (inString)
            {
                if (escaped)
                {
                    escaped = false;
                }
                else if (c == '\\')
                {
                    escaped = true;
                }
                else if (c == '"')
                {
                    inString = false;
                }

                continue;
            }

            if (c == '"')
            {
                inString = true;
            }
            else if (c == '{')
            {
                depth++;
            }
            else if (c == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return text[startIndex..(i + 1)];
                }
            }
        }

        return null;
    }

    private static SearchPage ParseListingLinks(string html)
    {
        var cars = Regex.Matches(
                html,
                @"href=[""'](?<url>[^""']*auto-occasion-annonce-[^""']+?\.html[^""']*)[""']",
                RegexOptions.IgnoreCase)
            .Select(match => WebUtility.HtmlDecode(match.Groups["url"].Value))
            .Select(NormalizeListingUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(url => new Car { Url = url! })
            .ToList();

        return new SearchPage(cars, ExtractTotalCarsFromHtml(html));
    }

    private static string? NormalizeListingUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return url;
        return url.StartsWith('/') ? $"{SiteBaseUrl}{url}" : $"{SiteBaseUrl}/{url}";
    }

    private static int? ExtractTotalCarsFromHtml(string html)
    {
        var text = WebUtility.HtmlDecode(Regex.Replace(html, "<.*?>", " "));
        var match = Regex.Match(text, @"\b(?<count>\d[\d\s]{0,8})\s+(?:annonces?|véhicules?|resultats?|résultats?)\b", RegexOptions.IgnoreCase);
        if (!match.Success) return null;

        var value = match.Groups["count"].Value.Replace(" ", "");
        return int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var count) ? count : null;
    }

    private static Car? ParseHit(JsonElement hit)
    {
        var item = hit.TryGetProperty("item", out var itemElement) ? itemElement : hit;
        var vehicle = item.TryGetProperty("vehicle", out var vehicleElement) ? vehicleElement : item;
        var url = ExtractUrl(item);
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        return new Car
        {
            Url = url,
            Phone = FindFirstString(item, "phone", "phoneNumber", "sellerPhone"),
            DealerName = FindFirstString(item, "customerName", "sellerName", "storeName", "companyName", "name"),
            Make = FindFirstString(vehicle, "make", "brand"),
            Model = FindFirstString(vehicle, "model", "commercialName"),
            Color = FindFirstString(vehicle, "externalColor", "color", "exteriorColor"),
            Price = FindFirstString(item, "price", "priceWithTax", "consumerPrice"),
            Kilometre = FindFirstString(vehicle, "mileage", "kilometers", "km"),
            RegistartionDate = FindFirstInt(vehicle, "year", "firstRegistrationYear"),
            AdvertisingDate = FindFirstDate(item, "firstOnlineDate", "publicationDate", "createdAt", "onlineDate")
        };
    }

    private static string? ExtractUrl(JsonElement item)
    {
        var url = FindFirstString(item, "url", "permalink", "publicUrl", "adUrl");
        if (!string.IsNullOrWhiteSpace(url))
        {
            return url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : $"{SiteBaseUrl}{url}";
        }

        var reference = FindFirstString(item, "reference", "id", "classifiedReference");
        return string.IsNullOrWhiteSpace(reference)
            ? null
            : $"{SiteBaseUrl}/auto-occasion-annonce-{reference}.html";
    }

    private static List<Car> ApplyPostFilters(List<Car> cars, LaCentraleInputModel input)
    {
        if (input.AdvertisingDate != null)
            cars = cars.Where(x => x.AdvertisingDate == null || x.AdvertisingDate >= input.AdvertisingDate).ToList();

        if (input.RegistrationYear is > 0)
            cars = cars.Where(x => x.RegistartionDate == null || x.RegistartionDate >= input.RegistrationYear).ToList();

        if (!string.IsNullOrWhiteSpace(input.CompanySeller))
            cars = cars.Where(x => x.DealerName != null &&
                                   x.DealerName.Replace(" ", "").Equals(
                                       input.CompanySeller.Replace(" ", ""),
                                       StringComparison.CurrentCultureIgnoreCase)).ToList();

        return cars;
    }

    private static string? FindFirstString(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (TryGetPropertyRecursive(root, name, out var value))
            {
                return ElementToString(value);
            }
        }

        return null;
    }

    private static int? FindFirstInt(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (!TryGetPropertyRecursive(root, name, out var value)) continue;
            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number)) return number;
            if (int.TryParse(ElementToString(value), NumberStyles.Any, CultureInfo.InvariantCulture, out number)) return number;
        }

        return null;
    }

    private static DateTime? FindFirstDate(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            var value = FindFirstString(root, name);
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
            {
                return date;
            }
        }

        return null;
    }

    private static bool TryGetPropertyRecursive(JsonElement root, string propertyName, out JsonElement value)
    {
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (property.NameEquals(propertyName))
                {
                    value = property.Value;
                    return true;
                }

                if (TryGetPropertyRecursive(property.Value, propertyName, out value))
                {
                    return true;
                }
            }
        }
        else if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                if (TryGetPropertyRecursive(item, propertyName, out value))
                {
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private static string? ElementToString(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => null
        };
    }

    private static int GetInputProgressPercentage(int inputIndex, int inputCount)
    {
        if (inputCount <= 0) return 100;
        return Math.Clamp((int)Math.Floor(100 * (inputIndex / (double)inputCount)), 0, 100);
    }

    private static int GetPageProgressPercentage(
        int startPercentage,
        int endPercentage,
        int page,
        int completedItems,
        int totalItems)
    {
        var span = Math.Max(1, endPercentage - startPercentage);
        var pageOffset = Math.Min(span - 1, Math.Max(0, page - 1) * 8);
        var itemProgress = totalItems <= 0
            ? 0
            : (int)Math.Floor(Math.Min(1, completedItems / (double)totalItems) * span);

        return Math.Clamp(startPercentage + Math.Max(pageOffset, itemProgress), startPercentage, endPercentage);
    }

    private static string Preview(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        text = text.ReplaceLineEndings(" ").Trim();
        return text.Length <= 180 ? text : text[..180];
    }

    private sealed record SearchPage(List<Car> Cars, int? TotalCars);
}
