using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
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
    private static readonly Regex CarIdRegex = new(@"(?<!\d)(\d{6,})(?!\d)", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"\+?\d[\d\s().-]{6,}\d", RegexOptions.Compiled);
    private LeBonCoinLoginResult? _loginResult;

    [Obsolete("Obsolete")]
    public async Task Start(
        List<LeboncoinInputModel> inputs,
        IProgress<(int Percentage, string Message)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory("leboncoin json outputs");
        Directory.CreateDirectory("leboncoin outputs");

        progress?.Report((2, "Solving Leboncoin DataDome challenge..."));
        await PhantomTLS.InitializeAsync();
        await using var client = new PhantomClient(new PhantomClientOptions
        {
            ClientIdentifier = "chrome_120",
            //Proxy = "http://wxxedufq:xt007td5f6dc@64.137.10.153:5803",
            Timeout = 30000,
            //Http2 = true
        });
        var leBonCoinLoginService = new LeBonCoinLoginService();
       
        await Task.Delay(10000, cancellationToken);
        // await leBonCoinLoginService.LogOut();
        // return;
        var cookies = await new DataDomeRaper().DataDomeCookieCatcher("https://www.leboncoin.fr/");
        // var cookies = await GetCookie(client, cancellationToken);
        ValidateCookies(cookies);
        progress?.Report((5, "Leboncoin DataDome cookies ready."));

        var cars = new List<Car>();
        var validInputs = inputs.Where(x => true).ToList();
        for (var i = 0; i < validInputs.Count; i++)
        {
            var input = validInputs[i];
            cancellationToken.ThrowIfCancellationRequested();
            var inputStartPercentage = GetInputProgressPercentage(i, validInputs.Count);
            var inputEndPercentage = GetInputProgressPercentage(i + 1, validInputs.Count);
            cars.AddRange(await StartScraping(
                client,
                cookies,
                input,
                inputStartPercentage,
                inputEndPercentage,
                progress,
                cancellationToken));
            await Task.Delay(1500, cancellationToken);
        }
        var json1 = JsonConvert.SerializeObject(cars, Formatting.Indented);
        await File.WriteAllTextAsync(
            $"leboncoin json outputs/leboncoin_cars_json_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.json",
            json1,
            cancellationToken);
        _loginResult = await leBonCoinLoginService.LoginAsync("bilelproscraper@gmail.com", "Bilou23051984@@",
            cancellationToken: cancellationToken);
        await PopulatePhoneNumbers(client, cars, progress, cancellationToken);

        var json = JsonConvert.SerializeObject(cars, Formatting.Indented);
        await File.WriteAllTextAsync(
            $"leboncoin json outputs/leboncoin_cars_json_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.json",
            json,
            cancellationToken);
        cars.SaveToExcel($"leboncoin outputs/leboncoin_cars_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx");
        await leBonCoinLoginService.LogOut();
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
        int startPercentage,
        int endPercentage,
        IProgress<(int Percentage, string Message)>? progress,
        CancellationToken cancellationToken)
    {
        var cars = new List<Car>();
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var page = 1;
        var carsFound = 0;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var url = BuildSearchDataUrl(cookies.route, input, page);
            var pageStartPercentage = GetPageProgressPercentage(startPercentage, endPercentage, page, 0, 1);
            progress?.Report((pageStartPercentage,
                $"Loading Leboncoin data page {page}: {input.Make?.Name}/{input.Model?.Name}"));
            var json = await GetString(client, url, cookies, cancellationToken);

            var searchPage = ExtractListingUrls(json);
            if (!searchPage.IsValidJson)
            {
                throw new InvalidOperationException(
                    $"Leboncoin returned a non-JSON response for {input.Make?.Name}/{input.Model?.Name}. Response starts with: {GetResponsePreview(json)}");
            }

            if (searchPage.ListingUrls.Count == 0)
            {
                progress?.Report((pageStartPercentage,
                    $"No Leboncoin listings found for {input.Make?.Name}/{input.Model?.Name} on page {page}."));
                break;
            }

            var newListingUrls = searchPage.ListingUrls
                .Where(listingUrl => !seenUrls.Contains(listingUrl))
                .ToList();
            carsFound += newListingUrls.Count;

            foreach (var listingUrl in searchPage.ListingUrls)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!seenUrls.Add(listingUrl)) continue;

                var car = await GetDetails(client, listingUrl, cookies, cancellationToken);
                if (car == null) continue;

                cars.Add(car);
                var percentage = GetPageProgressPercentage(
                    startPercentage,
                    endPercentage,
                    page,
                    cars.Count,
                    Math.Max(cars.Count, carsFound));
                progress?.Report((percentage,
                    $"{cars.Count} of {carsFound} Leboncoin cars scraped: {input.Make?.Name}/{input.Model?.Name}"));
            }

            if (!searchPage.HasPivot)
            {
                break;
            }

            page++;
        }

        cars = ApplyPostFilters(cars, input);
        progress?.Report((endPercentage, $"{cars.Count} of {carsFound} Leboncoin cars kept after filters."));
        return cars;
    }

    private static int GetInputProgressPercentage(int inputIndex, int inputCount)
    {
        if (inputCount <= 0) return 100;
        return Math.Clamp(5 + (int)Math.Floor(95 * (inputIndex / (double)inputCount)), 0, 100);
    }

    private static int GetPageProgressPercentage(
        int startPercentage,
        int endPercentage,
        int page,
        int completedItems,
        int totalItems)
    {
        var span = Math.Max(1, endPercentage - startPercentage);
        var pageOffset = Math.Min(span - 1, Math.Max(0, page - 1) * 10);
        var pageProgress = totalItems <= 0
            ? 0
            : (int)Math.Floor(Math.Min(1, completedItems / (double)totalItems) * 10);

        return Math.Clamp(startPercentage + Math.Min(span - 1, pageOffset + pageProgress), startPercentage,
            endPercentage);
    }

    private static void ValidateCookies((string dataDome, string SecureInstall, string route) cookies)
    {
        if (string.IsNullOrWhiteSpace(cookies.dataDome) ||
            string.IsNullOrWhiteSpace(cookies.SecureInstall) ||
            string.IsNullOrWhiteSpace(cookies.route) ||
            cookies.dataDome.Equals("not found", StringComparison.OrdinalIgnoreCase) ||
            cookies.SecureInstall.Equals("not found", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Leboncoin DataDome cookies or route were not captured correctly.");
        }
    }

    private static string GetResponsePreview(string response)
    {
        var clean = Regex.Replace(response, @"\s+", " ").Trim();
        return clean.Length <= 180 ? clean : clean[..180];
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
            var rnd = new Random();
            // var dealerId = rnd.Next(5000, 10000);
            // await Task.Delay(dealerId, cancellationToken);
            return new Car
            {
                Url = carUrl,
                DealerName = dealerName,
                Make = ExtractHtmlValue(doc, "Marque"),
                Model = ExtractHtmlValue(doc, "Modèle"),
                Color = ExtractHtmlValue(doc, "Couleur") ?? ExtractHtmlValue(doc, "Couleur extérieure"),
                Price = ExtractPrice(doc, visibleText),
                Kilometre = ExtractHtmlValue(doc, "Kilométrage") ?? ExtractHtmlValue(doc, "Kilomètres"),
                RegistartionDate = ExtractYear(ExtractHtmlValue(doc, "Année") ?? visibleText),
                AdvertisingDate = ExtractAdvertisingDate(doc, visibleText)
            };
        }
        catch
        {
            return null;
        }
    }

    private async Task PopulatePhoneNumbers(
        PhantomClient client,
        List<Car> cars,
        IProgress<(int Percentage, string Message)>? progress,
        CancellationToken cancellationToken)
    {
        if (cars.Count == 0)
        {
            return;
        }

        progress?.Report((95, $"Retrieving phone numbers for {cars.Count} Leboncoin cars..."));

        for (var i = 0; i < cars.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var car = cars[i];
            if (string.IsNullOrWhiteSpace(car.Url))
            {
                continue;
            }

            car.Phone = await GetPhoneNumber(client, car.Url, _loginResult, cancellationToken);
            //await Task.Delay(20000, cancellationToken);
            var percentage = 95 + (int)Math.Floor(4 * ((i + 1) / (double)cars.Count));
            progress?.Report((percentage, $"{i + 1} of {cars.Count} Leboncoin phone numbers retrieved."));
        }
    }

    private static async Task<string?> GetPhoneNumber(
        PhantomClient client,
        string carUrl,
        LeBonCoinLoginResult? loginResult,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var carId = ExtractCarId(carUrl);

        var accessToken = ExtractAccessToken(loginResult?.AuthorizerTokenResponse);
        var datadomeCookie = loginResult?.DatadomeCookie;

        if (string.IsNullOrWhiteSpace(carId) ||
            string.IsNullOrWhiteSpace(accessToken) ||
            string.IsNullOrWhiteSpace(datadomeCookie))
        {
            return null;
        }

        var phoneTrackingSessionId = Guid.NewGuid().ToString();
        var url =
            $"https://api.leboncoin.fr/api/call-tracking/v1/classified/{Uri.EscapeDataString(carId)}/phone" +
            $"?phoneTrackingSessionId={Uri.EscapeDataString(phoneTrackingSessionId)}&referrerType=alu";

        var tries = 0;
        try
        {
            do
            {
                var response = await client.GetAsync(url, new RequestOptions
                {
                    Headers = new Dictionary<string, string>
                    {
                        ["authorization"] = $"Bearer {accessToken}",
                        ["Cookie"] = $"datadome={datadomeCookie}",
                        ["User-Agent"] =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36"
                    }
                });

                // if (!response.IsSuccess || string.IsNullOrWhiteSpace(response.Body))
                // {
                //     return null;
                // }

                if (response.Status == 429)
                {
                    if (tries == 10)
                    {
                        return "";
                    }

                    tries++;
                    Console.WriteLine($"retry after 429 status code, try number {tries}");
                    await Task.Delay(60000, cancellationToken);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(response.Body))
                {
                    
                }
                Console.WriteLine($"phone number retrieved after {tries} tries.");
                if (response.Status == 404)
                {
                    return "N/A";
                }
                return ExtractPhoneNumberFromJson(response.Body);
            } while (true);
        }
        catch
        {
            return null;
        }
    }

    private static string? ExtractCarId(string carUrl)
    {
        if (Uri.TryCreate(carUrl, UriKind.Absolute, out var uri))
        {
            var pathMatch = CarIdRegex.Matches(uri.AbsolutePath).LastOrDefault();
            if (pathMatch?.Success == true)
            {
                return pathMatch.Groups[1].Value;
            }
        }

        var match = CarIdRegex.Matches(carUrl).LastOrDefault();
        return match?.Success == true ? match.Groups[1].Value : null;
    }

    private static string? ExtractAccessToken(string? tokenResponse)
    {
        if (string.IsNullOrWhiteSpace(tokenResponse))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(tokenResponse);
            return doc.RootElement.TryGetProperty("access_token", out var accessTokenElement)
                ? accessTokenElement.GetString()
                : null;
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }

    private static string? ExtractPhoneNumberFromJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return FindPhoneNumber(doc.RootElement);
        }
        catch (System.Text.Json.JsonException)
        {
            var match = PhoneRegex.Match(json);
            return match.Success ? match.Value.Trim() : null;
        }
    }

    private static string? FindPhoneNumber(JsonElement element, string? propertyName = null)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var value = FindPhoneNumber(property.Value, property.Name);
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }

                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    var value = FindPhoneNumber(item, propertyName);
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }

                break;
            case JsonValueKind.String:
                var text = element.GetString();
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                if (propertyName?.Contains("phone", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return text.Trim();
                }

                var match = PhoneRegex.Match(text);
                return match.Success ? match.Value.Trim() : null;
        }

        return null;
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
            ["horse_power_din"] = BuildRange(input.DinPowerFrom, input.DinPowerTo),
            ["regdate"] = $"{input.RegistrationYear}-max"
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

    private sealed record SearchPageResult(List<string> ListingUrls, bool HasPivot, bool IsValidJson);

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

            return new SearchPageResult(listingUrls, hasPivot, true);
        }
        catch
        {
            return new SearchPageResult([], false, false);
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

        // if (int.TryParse(input.RegistrationYear, out var registrationYear) && registrationYear > 0)
        //     cars = cars.Where(x => x.RegistartionDate == null || x.RegistartionDate >= registrationYear).ToList();

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

    private static string? ExtractPrice(HtmlDocument doc, string text)
    {
        var priceNode = doc.DocumentNode.SelectSingleNode("//*[@data-qa-id='adview_price']//*[contains(text(), '€')]") ??
                        doc.DocumentNode.SelectSingleNode("//*[@data-qa-id='adview_price']");
        var price = NormalizeEuroPrice(priceNode?.InnerText);
        if (!string.IsNullOrWhiteSpace(price))
        {
            return price;
        }

        var scriptNodes = doc.DocumentNode.SelectNodes("//script[@type='application/ld+json']");
        if (scriptNodes != null)
        {
            foreach (var scriptNode in scriptNodes)
            {
                price = ExtractJsonLdPrice(scriptNode.InnerText);
                if (!string.IsNullOrWhiteSpace(price))
                {
                    return price;
                }
            }
        }

        return NormalizeEuroPrice(text);
    }

    private static string? ExtractJsonLdPrice(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(WebUtility.HtmlDecode(json));
            return FindOfferPrice(document.RootElement);
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }

    private static string? FindOfferPrice(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty("offers", out var offers))
            {
                var offerPrice = FindOfferPrice(offers);
                if (!string.IsNullOrWhiteSpace(offerPrice))
                {
                    return offerPrice;
                }
            }

            if (element.TryGetProperty("price", out var price))
            {
                return price.ValueKind switch
                {
                    JsonValueKind.Number when price.TryGetDecimal(out var value) => FormatEuroPrice(value),
                    JsonValueKind.String => NormalizeEuroPrice(price.GetString()),
                    _ => null
                };
            }

            foreach (var property in element.EnumerateObject())
            {
                var value = FindOfferPrice(property.Value);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                var value = FindOfferPrice(item);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }

        return null;
    }

    private static string? NormalizeEuroPrice(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var clean = CleanText(text).Replace('\u00A0', ' ').Replace('\u202F', ' ');
        var match = Regex.Match(clean, @"(?<!\d)(\d{1,3}(?:[\s.]+\d{3})*|\d+)\s*(?:€|EUR)\b", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            return null;
        }

        var amount = Regex.Replace(match.Groups[1].Value, @"[\s.]+", " ").Trim();
        return $"{amount} €";
    }

    private static string FormatEuroPrice(decimal value)
    {
        var amount = value.ToString("N0", CultureInfo.GetCultureInfo("fr-FR"))
            .Replace('\u00A0', ' ')
            .Replace('\u202F', ' ');
        return $"{amount} €";
    }

    private static string? ExtractLineAfter(string text, string marker)
    {
        var lines = text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var index = Array.FindIndex(lines, x => x.Contains(marker, StringComparison.OrdinalIgnoreCase));
        return index >= 0 && index + 1 < lines.Length ? lines[index + 1] : null;
    }

    private static DateTime? ExtractAdvertisingDate(HtmlDocument doc, string visibleText)
    {
        var priceNode = doc.DocumentNode.SelectSingleNode("//*[@data-qa-id='adview_price']//*[contains(text(), '€')]") ??
                        doc.DocumentNode.SelectSingleNode("//*[@data-qa-id='adview_price']");

        var followingDate = ExtractFirstDateAfterPriceNode(priceNode);
        if (followingDate != null)
        {
            return followingDate;
        }

        var priceContainer = doc.DocumentNode.SelectSingleNode("//*[@data-qa-id='adview_price']/ancestor::section[1]") ??
                             doc.DocumentNode.SelectSingleNode("//*[@data-qa-id='adview_price']/ancestor::article[1]") ??
                             doc.DocumentNode.SelectSingleNode("//*[@data-qa-id='adview_price']/ancestor::div[4]");

        var date = ParseAdvertisingDate(ExtractTextAfterPrice(priceContainer?.InnerText, priceNode?.InnerText));
        if (date != null)
        {
            return date;
        }

        return ParseAdvertisingDate(ExtractTextAfterPrice(visibleText, priceNode?.InnerText));
    }

    private static DateTime? ExtractFirstDateAfterPriceNode(HtmlNode? priceNode)
    {
        var nodes = priceNode?.SelectNodes("following::*[not(self::script) and not(self::style)]");
        if (nodes == null)
        {
            return null;
        }

        foreach (var node in nodes.Take(30))
        {
            var date = ParseAdvertisingDate(node.InnerText);
            if (date != null)
            {
                return date;
            }
        }

        return null;
    }

    private static string? ExtractTextAfterPrice(string? sourceText, string? priceText)
    {
        if (string.IsNullOrWhiteSpace(sourceText))
        {
            return null;
        }

        var cleanSource = NormalizeWhitespace(sourceText);
        var normalizedPrice = NormalizeEuroPrice(priceText);
        if (string.IsNullOrWhiteSpace(normalizedPrice))
        {
            return cleanSource;
        }

        var index = cleanSource.IndexOf(normalizedPrice, StringComparison.OrdinalIgnoreCase);
        return index < 0 ? cleanSource : cleanSource[(index + normalizedPrice.Length)..];
    }

    private static DateTime? ParseAdvertisingDate(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var clean = NormalizeWhitespace(text);
        var relativeMatch = Regex.Match(clean, @"\b(aujourd[’']hui|hier)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (relativeMatch.Success)
        {
            var relativeDate = relativeMatch.Groups[1].Value.StartsWith("hier", StringComparison.OrdinalIgnoreCase)
                ? DateTime.Today.AddDays(-1)
                : DateTime.Today;
            return ApplyAdvertisingTime(relativeDate, clean);
        }

        var numericMatch = Regex.Match(clean, @"\b\d{1,2}/\d{1,2}/\d{4}\b");
        if (numericMatch.Success &&
            DateTime.TryParseExact(
                numericMatch.Value,
                ["d/M/yyyy", "dd/MM/yyyy"],
                CultureInfo.GetCultureInfo("fr-FR"),
                DateTimeStyles.None,
                out var numericDate))
        {
            return ApplyAdvertisingTime(numericDate, clean);
        }

        var monthMatch = Regex.Match(
            clean,
            @"\b\d{1,2}\s+(?:janvier|février|fevrier|mars|avril|mai|juin|juillet|août|aout|septembre|octobre|novembre|décembre|decembre)\s+\d{4}\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!monthMatch.Success)
        {
            return null;
        }

        var frenchDate = RemoveDiacritics(monthMatch.Value.ToLowerInvariant());
        return DateTime.TryParseExact(
            frenchDate,
            "d MMMM yyyy",
            CultureInfo.GetCultureInfo("fr-FR"),
            DateTimeStyles.None,
            out var monthDate)
            ? ApplyAdvertisingTime(monthDate, clean)
            : null;
    }

    private static DateTime ApplyAdvertisingTime(DateTime date, string text)
    {
        var timeMatch = Regex.Match(text, @"\b(?:à|a)\s*(\d{1,2})(?::|\s*h(?:eures?)?\s*)(\d{2})\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!timeMatch.Success)
        {
            return date.Date;
        }

        var hour = int.Parse(timeMatch.Groups[1].Value, CultureInfo.InvariantCulture);
        var minute = int.Parse(timeMatch.Groups[2].Value, CultureInfo.InvariantCulture);
        return hour is >= 0 and <= 23 && minute is >= 0 and <= 59
            ? date.Date.AddHours(hour).AddMinutes(minute)
            : date.Date;
    }

    private static string NormalizeWhitespace(string text) =>
        CleanText(text).Replace('\u00A0', ' ').Replace('\u202F', ' ');

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
