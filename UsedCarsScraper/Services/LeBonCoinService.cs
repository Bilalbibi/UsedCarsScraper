using System.Globalization;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using ExcelHelperEx;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PhantomClientCore;
using UsedCarsScraper.Extensions;
using UsedCarsScraper.GeneralModels;
using UsedCarsScraper.LeBonCoinModels;

namespace UsedCarsScraper.Services;

public class LeBonCoinService
{
    private const string BaseUrl = "https://www.leboncoin.fr";
    private static readonly Regex YearRegex = new(@"\b(?:19|20)\d{2}\b", RegexOptions.Compiled);

    [Obsolete("Obsolete")]
    public async Task Start(
        List<LeboncoinInputModel> inputs,
        IProgress<(int Percentage, string Message)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory("leboncoin json outputs");
        Directory.CreateDirectory("leboncoin outputs");

        progress?.Report((0, "Solving Leboncoin DataDome challenge..."));
        await PhantomTLS.InitializeAsync();
        await using var client = new PhantomClient(new PhantomClientOptions
        {
            ClientIdentifier = "chrome_120",
            //Proxy = "http://wxxedufq:xt007td5f6dc@64.137.10.153:5803",
            Timeout = 30000,
            //Http2 = true
        });
       var cookies = await new DataDomeRaper().DataDomeCookieCatcher();
       // var cookies = await GetCookie(client, cancellationToken);

        var cars = new List<Car>();
        foreach (var input in inputs.Where(x => true))
        {
            cancellationToken.ThrowIfCancellationRequested();
            cars.AddRange(await StartScraping(client, cookies, input, progress, cancellationToken));
            await Task.Delay(1500, cancellationToken);
        }

        var json = JsonConvert.SerializeObject(cars, Formatting.Indented);
        await File.WriteAllTextAsync(
            $"leboncoin json outputs/leboncoin_cars_json_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.json",
            json,
            cancellationToken);
        cars.SaveToExcel($"leboncoin outputs/leboncoin_cars_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx");
    }

    private async Task<(string dataDome, string SecureInstall, string route)> GetCookie(PhantomClient client
        , CancellationToken cancellationToken)
    {
        var response = await client.GetAsync("https://www.leboncoin.fr/", new RequestOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
                ["User-Agent"] =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
                ["Accept-Language"] = "en-US,en;q=0.9"
            },
        });
        var doc = new HtmlDocument();
        doc.LoadHtml(response.Body);
        var route = doc.DocumentNode.SelectSingleNode("//script[@defer][last()]").GetAttributeValue("src", "");
        route = route.Replace("/_next/static/", "");
        var x = route.LastIndexOf('/');
        route = route[..x];
        var dataDome = response.Cookies["datadome"];
        var secureInstall = response.Cookies["__Secure-Install"];
        return (dataDome, secureInstall, route);
    }

    private async Task<List<Car>> StartScraping(
        PhantomClient client,
        (string dataDome, string SecureInstall, string route) cookies,
        LeboncoinInputModel input,
        IProgress<(int Percentage, string Message)>? progress,
        CancellationToken cancellationToken)
    {
        var cars = new List<Car>();
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var page = 1;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var url = BuildSearchDataUrl(cookies.route, input, page);
            progress?.Report((0, $"Loading Leboncoin data page {page}: {input.Make?.Name}/{input.Model?.Name}"));
            var json = await GetString(client, url, cookies, cancellationToken);
            if (!json.Contains("leboncoin, site de petites annonces gratuites"))
            {
                 cookies = await new DataDomeRaper().DataDomeCookieCatcher();
                 json = await GetString(client, url, cookies, cancellationToken);
            }
            var searchPage = ExtractListingUrls(json);
            if (searchPage.ListingUrls.Count == 0)
            {
                break;
            }

            foreach (var listingUrl in searchPage.ListingUrls)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!seenUrls.Add(listingUrl)) continue;

                var car = await GetDetails(client, listingUrl, cookies, cancellationToken);
                if (car == null) continue;

                cars.Add(car);
                progress?.Report((0, $"{cars.Count} Leboncoin cars scraped: {input.Make?.Name}/{input.Model?.Name}"));
            }

            if (!searchPage.HasPivot)
            {
                break;
            }

            page++;
        }

        cars = ApplyPostFilters(cars, input);
        progress?.Report((0, $"Leboncoin cars found after filters: {cars.Count}"));
        return cars;
    }

    public async Task<Car?> GetDetails(
        PhantomClient client,
        string carUrl,
        (string dataDome, string SecureInstall, string route) cookies,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var html = await GetString(client, carUrl, cookies, cancellationToken);
            var doc = html.ToDoc();
            var visibleText = WebUtility.HtmlDecode(doc.DocumentNode.InnerText);
            var dealerName = ExtractHtmlValue(doc, "Vendeur") ??
                             ExtractHtmlValue(doc, "Nom du vendeur") ??
                             ExtractDealerNameFromNextData(doc);

            return new Car
            {
                Url = carUrl,
                Phone = null,
                DealerName = dealerName,
                Make = ExtractHtmlValue(doc, "Marque"),
                Model = ExtractHtmlValue(doc, "Modèle"),
                Color = ExtractHtmlValue(doc, "Couleur") ?? ExtractHtmlValue(doc, "Couleur extérieure"),
                Price = ExtractPrice(visibleText),
                Kilometre = ExtractHtmlValue(doc, "Kilométrage") ?? ExtractHtmlValue(doc, "Kilomètres"),
                RegistartionDate = ExtractYear(ExtractHtmlValue(doc, "Année") ?? visibleText),
                AdvertisingDate = ExtractAdvertisingDate(visibleText)
            };
        }
        catch
        {
            return null;
        }
    }

    private static async Task<string> GetString(
        PhantomClient client,
        string url,
        (string dataDome, string SecureInstall, string route) cookies,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = await client.GetAsync(url, new RequestOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Accept"] =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
                ["cookie"] =
                    $"__Secure-Install={cookies.SecureInstall}; datadome={cookies.dataDome}",
                ["sec-fetch-mode"] = "cors",
                ["User-Agent"] =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
                ["Accept-Language"] = "en-US,en;q=0.9"
            },
        });

        return response.Body;
    }

    private static string BuildSearchDataUrl(string route, LeboncoinInputModel input, int page)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["category"] = "2",
            ["order"] = "desc",
            ["page"] = page > 1 ? page.ToString(CultureInfo.InvariantCulture) : null,
            ["u_car_brand"] = input.Make?.Value ?? input.Make?.Name,
            ["u_car_model"] = input.Model?.Value,
            ["fuel"] = input.FuelType?.Value,
            ["price"] = BuildRange(input.PriceFrom, input.PriceTo),
            ["mileage"] = BuildRange(input.MileAgeFrom, input.MileAgeTo),
            ["horse_power_din"] = BuildRange(input.DinPowerFrom, input.DinPowerTo)
        };

        var query = string.Join("&", parameters
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value!)}"));

        return $"{BaseUrl}/_next/data/{route}/recherche.json?{query}";
    }

    private static string? BuildRange(string? from, string? to)
    {
        if (string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to)) return null;
        var min = string.IsNullOrWhiteSpace(from) ? "min" : from.Trim();
        var max = string.IsNullOrWhiteSpace(to) ? "max" : to.Trim();
        return $"{min}-{max}";
    }

    private sealed record SearchPageResult(List<string> ListingUrls, bool HasPivot);

    private static SearchPageResult ExtractListingUrls(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var listingUrls = EnumerateStrings(doc.RootElement)
                .Where(x => x.Contains("/ad/voitures/", StringComparison.OrdinalIgnoreCase) ||
                            x.Contains("/voitures/", StringComparison.OrdinalIgnoreCase))
                .Select(NormalizeUrl)
                .Where(x => !string.IsNullOrWhiteSpace(x) &&
                            !x.Contains("/recherche", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var hasPivot = TryFindProperty(doc.RootElement, "pivot", out var pivot) && !IsEmptyJsonValue(pivot);

            return new SearchPageResult(listingUrls, hasPivot);
        }
        catch
        {
            return new SearchPageResult([], false);
        }
    }

    private static bool TryFindProperty(JsonElement element, string propertyName, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals(propertyName))
                {
                    value = property.Value;
                    return true;
                }

                if (TryFindProperty(property.Value, propertyName, out value))
                {
                    return true;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                if (TryFindProperty(item, propertyName, out value))
                {
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private static bool IsEmptyJsonValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => true,
            JsonValueKind.String => string.IsNullOrWhiteSpace(element.GetString()),
            JsonValueKind.Array => !element.EnumerateArray().Any(),
            JsonValueKind.Object => !element.EnumerateObject().Any(),
            _ => false
        };
    }

    private static IEnumerable<string> EnumerateStrings(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                var value = element.GetString();
                if (!string.IsNullOrWhiteSpace(value)) yield return value;
                break;
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                foreach (var child in EnumerateStrings(property.Value))
                    yield return child;
                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                foreach (var child in EnumerateStrings(item))
                    yield return child;
                break;
        }
    }

    private static string NormalizeUrl(string url)
    {
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return url;
        return url.StartsWith('/') ? $"{BaseUrl}{url}" : $"{BaseUrl}/{url}";
    }

    private static string? ExtractHtmlValue(HtmlDocument doc, string label)
    {
        var labelNode = doc.DocumentNode
            .Descendants()
            .FirstOrDefault(node => IsVisibleElement(node) && TextEquals(node.InnerText, label));
        if (labelNode == null) return ExtractLineAfter(WebUtility.HtmlDecode(doc.DocumentNode.InnerText), label);

        var siblingValue = labelNode
            .SelectNodes("following-sibling::*")
            ?.Select(node => CleanText(node.InnerText))
            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text) && !TextEquals(text, label));
        if (!string.IsNullOrWhiteSpace(siblingValue)) return siblingValue;

        foreach (var container in labelNode.Ancestors().Take(4))
        {
            var value = container
                .Descendants()
                .Where(IsVisibleElement)
                .Select(node => CleanText(node.InnerText))
                .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text) && !TextEquals(text, label));
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }

        return ExtractLineAfter(WebUtility.HtmlDecode(doc.DocumentNode.InnerText), label);
    }

    private static string? ExtractDealerNameFromNextData(HtmlDocument doc)
    {
        try
        {
            var script = doc.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']")?.InnerText;
            if (string.IsNullOrWhiteSpace(script)) return null;

            using var json = JsonDocument.Parse(WebUtility.HtmlDecode(script));
            return FindFirst(json, "store_name", "company_name", "dealer_name", "seller_name", "name");
        }
        catch
        {
            return null;
        }
    }

    private static string? FindFirst(JsonDocument doc, params string[] names)
    {
        foreach (var element in EnumerateObjects(doc.RootElement))
        {
            foreach (var name in names)
            {
                if (!element.TryGetProperty(name, out var property)) continue;
                var value = ElementToString(property);
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }
        }

        return null;
    }

    private static IEnumerable<JsonElement> EnumerateObjects(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            yield return element;
            foreach (var property in element.EnumerateObject())
            foreach (var child in EnumerateObjects(property.Value))
                yield return child;
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            foreach (var child in EnumerateObjects(item))
                yield return child;
        }
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

    private static bool IsVisibleElement(HtmlNode node)
    {
        if (node.NodeType != HtmlNodeType.Element) return false;
        if (node.Name is "script" or "style" or "noscript") return false;
        return !string.IsNullOrWhiteSpace(CleanText(node.InnerText));
    }

    private static bool TextEquals(string text, string expected) =>
        string.Equals(CleanText(text), expected, StringComparison.OrdinalIgnoreCase);

    private static string CleanText(string text) =>
        WebUtility.HtmlDecode(Regex.Replace(text, @"\s+", " ")).Trim();

    private static List<Car> ApplyPostFilters(List<Car> cars, LeboncoinInputModel input)
    {
        if (input.AdvertisingDate != null)
            cars = cars.Where(x => x.AdvertisingDate == null || x.AdvertisingDate >= input.AdvertisingDate).ToList();

        if (int.TryParse(input.RegistrationYear, out var registrationYear) && registrationYear > 0)
            cars = cars.Where(x => x.RegistartionDate == null || x.RegistartionDate >= registrationYear).ToList();

        if (!string.IsNullOrWhiteSpace(input.CompanySeller))
            cars = cars.Where(x => x.DealerName != null &&
                                   x.DealerName.Replace(" ", "").Equals(
                                       input.CompanySeller.Replace(" ", ""),
                                       StringComparison.CurrentCultureIgnoreCase)).ToList();

        return cars;
    }

    private static int? ExtractYear(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var match = YearRegex.Match(input);
        return match.Success ? int.Parse(match.Value, CultureInfo.InvariantCulture) : null;
    }

    private static string? ExtractPrice(string text)
    {
        var match = Regex.Match(text, @"\b\d[\d\s]*(?:€|EUR)\b", RegexOptions.IgnoreCase);
        return match.Success ? match.Value.Trim() : null;
    }

    private static string? ExtractLineAfter(string text, string marker)
    {
        var lines = text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var index = Array.FindIndex(lines, x => x.Contains(marker, StringComparison.OrdinalIgnoreCase));
        return index >= 0 && index + 1 < lines.Length ? lines[index + 1] : null;
    }

    private static DateTime? ExtractAdvertisingDate(string visibleText)
    {
        var match = Regex.Match(visibleText, @"\b\d{1,2}/\d{1,2}/\d{4}\b");
        return match.Success && DateTime.TryParse(match.Value, CultureInfo.GetCultureInfo("fr-FR"), out var parsed)
            ? parsed
            : null;
    }
}