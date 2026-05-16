using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ExcelHelperEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using UsedCarsScraper.Extensions;
using UsedCarsScraper.GeneralModels;
using UsedCarsScraper.StandVirtualModels;
namespace UsedCarsScraper.Services;

public class StandVirtualService
{
    HttpClient _client = new();
    private const string _baseUrl = "https://www.standvirtual.com/carros";
    private static readonly Regex YearRegex = new Regex(
        @"\b(?:19|20)\d{2}\b", 
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );
    public async Task Start(List<StandVirtualInputModel> inputs, IProgress<(int Percentage, string Message)> progress = null, CancellationToken cancellationToken = default)
    {
        var cars = new List<Car>();
        foreach (var input in inputs)
        {
            // Check for cancellation before processing next input
            cancellationToken.ThrowIfCancellationRequested();

            cars.AddRange(await StartScraping(input, progress, cancellationToken));
        
            // Pass token to delay
            await Task.Delay(2000, cancellationToken);
            progress?.Report((0, ""));
        }
        
        var json = JsonConvert.SerializeObject(cars, Formatting.Indented);
        await File.WriteAllTextAsync($"Standvirtual json outputs/Standvirtual_cars_json_{DateTime.Now:dd_MM_yyyy_mm_ss}.json", json, cancellationToken);
        cars.SaveToExcel($"Standvirtual outputs/Standvirtual_cars_{DateTime.Now:dd_MM_yyyy_mm_ss}.xlsx");
    }

    private async Task<List<Car>> StartScraping(StandVirtualInputModel standVirtualInput, IProgress<(int Percentage, string Message)> progress = null, CancellationToken cancellationToken = default)
    {
        var querySb = new StringBuilder();
        querySb.Append($"/{standVirtualInput.StandVirtualMake.Name?.ToLower()}/");

        if (!string.IsNullOrWhiteSpace(standVirtualInput.FromDate))
        {
            querySb.Append($"desde-{standVirtualInput.FromDate}");
        }

        if (!string.IsNullOrWhiteSpace(standVirtualInput.ToDate))
        {
            querySb.Append($"&search%5Bfilter_float_first_registration_year%3Ato%5D={standVirtualInput.ToDate}");
        }

        querySb.Append($"?search%5Bfilter_enum_fuel_type%5D={standVirtualInput.FuelType}");
        if (standVirtualInput.StandVirtualModel != null)
        {
            var modelName = standVirtualInput.StandVirtualModel.Name?.ToLower().Replace(" ", "-");
            modelName = RemoveDiacritics(modelName);
            querySb.Append($"&search%5Bfilter_enum_engine_code%5D={modelName}");
        }

        if (!string.IsNullOrWhiteSpace(standVirtualInput.BatteryCapacityFrom))
        {
            querySb.Append($"&search%5Bfilter_float_battery_capacity%3Afrom%5D={standVirtualInput.BatteryCapacityFrom}");
        }

        if (!string.IsNullOrWhiteSpace(standVirtualInput.BatteryCapacityTo))
        {
            querySb.Append($"&search%5Bfilter_float_battery_capacity%3Ato%5D={standVirtualInput.BatteryCapacityTo}");
        }

        if (!string.IsNullOrWhiteSpace(standVirtualInput.PriceFrom))
        {
            querySb.Append($"&search%5Bfilter_float_price%3Afrom%5D={standVirtualInput.PriceFrom}");
        }

        if (!string.IsNullOrWhiteSpace(standVirtualInput.PriceTo))
        {
            querySb.Append($"&search%5Bfilter_float_price%3Ato%5D={standVirtualInput.PriceTo}");
        }

        if (!string.IsNullOrWhiteSpace(standVirtualInput.MileAgeFrom))
        {
            querySb.Append($"&search%5Bfilter_float_mileage%3Afrom%5D={standVirtualInput.MileAgeFrom}");
        }

        if (!string.IsNullOrWhiteSpace(standVirtualInput.MileAgeTo))
        {
            querySb.Append($"&search%5Bfilter_float_mileage%3Ato%5D={standVirtualInput.MileAgeTo}");
        }

        if (standVirtualInput.Vat)
        {
            querySb.Append($"&search%5Bfilter_enum_tax_deductible%5D=1");
        }

        var page = 1;
        var pages = 1;
        var query = querySb.ToString();
        var url = $"{_baseUrl}{query}&page={page}";

        var response = await _client.Get(url, 5, [
            new KeyValuePair<string, string>("user-agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36")
        ]);
        var doc = response.Text.ToDoc();
        var numberOfCars = doc.DocumentNode.SelectSingleNode("//p[@class='elumsi70 ooa-1h4mewe']").InnerText.Trim();
        var array = numberOfCars.Split(' ');
        numberOfCars = array[0];
        var filters = "";
        var inputB = new StringBuilder();
        inputB.Append($"{standVirtualInput.StandVirtualMake}/");
        inputB.Append($"{standVirtualInput.StandVirtualModel}/");
        if (!string.IsNullOrEmpty(standVirtualInput.FromDate))
        {
            inputB.Append($"{standVirtualInput.FromDate}/");
        }

        if (!string.IsNullOrEmpty(standVirtualInput.ToDate))
        {
            inputB.Append($"{standVirtualInput.ToDate}/");
        }

        if (!string.IsNullOrEmpty(standVirtualInput.PriceFrom))
        {
            inputB.Append($"{standVirtualInput.PriceFrom}/");
        }

        if (!string.IsNullOrEmpty(standVirtualInput.PriceTo))
        {
            inputB.Append($"{standVirtualInput.PriceTo}/");
        }

        if (!string.IsNullOrEmpty(standVirtualInput.MileAgeFrom))
        {
            inputB.Append($"{standVirtualInput.MileAgeFrom}/");
        }

        if (!string.IsNullOrEmpty(standVirtualInput.MileAgeTo))
        {
            inputB.Append($"{standVirtualInput.MileAgeTo}/");
        }

        if (!string.IsNullOrEmpty(standVirtualInput.BatteryCapacityFrom))
        {
            inputB.Append($"{standVirtualInput.BatteryCapacityFrom}/");
        }

        if (!string.IsNullOrEmpty(standVirtualInput.BatteryCapacityTo))
        {
            inputB.Append($"{standVirtualInput.BatteryCapacityTo}/");
        }
        
        if (standVirtualInput.Vat)
        {
            inputB.Append($"vat selected/");
        }

        if (numberOfCars == "0")
        {
            
            filters = inputB.ToString();
            progress.Report((0, $"No results found for this filers: {filters}"));
            return [];
        }

        int.TryParse(numberOfCars.Replace(" ", ""), out var totalCars);

        var numberOfPagesText = doc.DocumentNode
            .SelectSingleNode("//button[@title='Go to next Page']/../preceding-sibling::li[1]")?.InnerText.Trim();
        if (!string.IsNullOrWhiteSpace(numberOfPagesText))
        {
            pages = int.Parse(numberOfPagesText);
        }

        var cars = new List<Car>();
        for (var i = 0; i < pages; i++)
        {
            cancellationToken.ThrowIfCancellationRequested(); 
            url = $"{_baseUrl}{query}&page={page}";
            response = await _client.Get(url, 5, [
                new KeyValuePair<string, string>("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36")
            ]);
            doc = response.Text.ToDoc();
            var nodes = doc.DocumentNode.SelectNodes(
                "//div[@data-testid='search-results']/article[@data-id]/section//a[@target='_self']");

            var carsUrl = nodes.Select(n => n.Attributes["href"].Value).ToList();

            foreach (var carUrl in carsUrl)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var car = await GetDetails(carUrl, cancellationToken);
                cars.Add(car);
                var message = $"{cars.Count} cars scraped/{numberOfCars} ==> input: {standVirtualInput.StandVirtualMake}/{standVirtualInput.StandVirtualModel}";
                if (progress == null || totalCars <= 0) continue;
                var percentage = (cars.Count * 100) / totalCars;
                progress.Report((percentage, message));
            }

            page++;
        }
        inputB = new StringBuilder();
        if (standVirtualInput.AdvertisingDate != null)
        {
            var filteredCars = cars.Where(x => x.AdvertisingDate != null && x.AdvertisingDate >= standVirtualInput.AdvertisingDate)
                .ToList();
            cars = filteredCars;
            var date= standVirtualInput.AdvertisingDate.Value.ToString("dd/MM/yyyy");
            inputB.Append($"Advertising date: {date}/");
        }

        if (standVirtualInput.RegistrationYear != null)
        {
            inputB.Append($"Registration year: {standVirtualInput.RegistrationYear}/");
        }

        if (!string.IsNullOrEmpty(standVirtualInput.CompanySeller))
        {
            inputB.Append($"supplier: {standVirtualInput.CompanySeller}/");
        }
        filters=inputB.ToString();
        progress?.Report((0, $"Mains cars found  {cars.Count} without activation those filters: {filters}"));
        if (!string.IsNullOrEmpty(standVirtualInput.CompanySeller))
        {
            var filteredCars = cars.Where(x => x.DealerName != null && x.DealerName.Replace(" ", "")
                .Equals(standVirtualInput.CompanySeller.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase)).ToList();
            cars = filteredCars;
        }

        // if (input.AdvertisingDate != null);
        // {
        //     var filteredCars = cars.Where(x => x.AdvertisingDate != null && x.AdvertisingDate >= input.AdvertisingDate)
        //         .ToList();
        //     cars = filteredCars;
        // }
        if (standVirtualInput.RegistrationYear > 0)
        {
            var filteredCars = cars.Where(x => x.RegistartionDate >= standVirtualInput.RegistrationYear)
                .ToList();
            cars = filteredCars;
        }

        progress?.Report((0, $"cars found {cars.Count} after activation those filters: {filters}"));
        return cars;
    }

    public async Task<Car> GetDetails(string carUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Car? car = null;
        try
        {
            var response = await _client.Get(carUrl, 5,
            [
                new KeyValuePair<string, string>("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36")
            ]);
            var doc = response.Text.ToDoc();
            var advertId = doc.DocumentNode.SelectSingleNode("//p[contains(text(),'ID')]").InnerText.Trim()
                .Replace("ID", "").Replace(":", "").Replace("\"", "").Trim();
            var script = doc.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']").InnerText.Trim();
            var x = script.IndexOf("\"uuid\":", StringComparison.Ordinal);
            var xx = script.IndexOf("\"equipment\"", StringComparison.Ordinal) - 2;
            var uuid = script.Substring(x, xx - x);
            uuid = uuid.Replace("\"", "").Replace("uuid:", "");
            var standId = "";
            //check if standId exist to add it to the payload
            if (script.Contains("standID", StringComparison.CurrentCultureIgnoreCase)) //message
            {
                x = script.IndexOf("\"standId\":\"", StringComparison.Ordinal);
                xx = script.IndexOf("\"uuid\":", StringComparison.Ordinal);
                standId = script.Substring(x, xx - x);
                standId = standId.Replace("\"standId\":\"", "").Replace(",", "").Replace("\"", "");
            }

            var variables = $"{{\"advertID\":\"{advertId}\",\"sellerUUID\":\"{uuid}\",\"touchPointPage\":\"ad_page\"}}";
            var extensions =
                "{\"persistedQuery\":{\"sha256Hash\":\"7c63f79d140155df49d0bf61080e134cf307c683db89dbde274a70fac6759e32\",\"version\":1}}";
            if (!string.IsNullOrEmpty(standId))
            {
                variables =
                    $"{{\"advertID\":\"{advertId}\",\"sellerUUID\":\"{uuid}\",\"standID\":\"{standId}\",\"touchPointPage\":\"ad_page\"}}";
            }

            var baseUrl = "https://www.standvirtual.com/graphql";
            var operationName = "getVirtualNumber";
            var encodedVariables = WebUtility.UrlEncode(variables);
            var encodedExtensions = WebUtility.UrlEncode(extensions);
            var finalUrl = $"{baseUrl}?operationName={operationName}" + $"&variables={encodedVariables}" +
                           $"&extensions={encodedExtensions}";
            var json = (await _client.Get(finalUrl, 5,
            [
                new KeyValuePair<string, string>("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36")
            ])).Text;
            var encrypted = "";
            var phone = "";
            if (json.Contains("message"))
            {
                var pageData = JObject.Parse(script);
                var phoneNumbersToken = pageData["props"]["pageProps"]["advert"]["phoneNumbers"];
                var encryptedNumbers = phoneNumbersToken.ToObject<List<string>>();

                if (encryptedNumbers.Count > 0)
                {
                    var phoneBld = new StringBuilder();
                    foreach (var encryptedPhone in encryptedNumbers)
                    {
                        try
                        {
                            // Pass it to the BouncyCastle decryptor we made earlier
                            var decryptedPhone = await DecryptAsync(encryptedPhone, standId);
                            phoneBld.Append($"{decryptedPhone}\r\n");
                        }
                        catch (Exception ex)
                        {
                            var decryptedPhone = await DecryptAsync(encryptedPhone, uuid);
                            phoneBld.Append($"{decryptedPhone}\r\n");
                            Console.WriteLine($"Failed to decrypt {encryptedPhone}: {ex.Message}");
                        }
                    }

                    phoneBld.Length--;
                    phoneBld.Length--;
                    phone = phoneBld.ToString();
                }
            }
            else
            {
                var phoneRespModel = JsonConvert.DeserializeObject<CarNumberEncodedResp>(json);
                encrypted = phoneRespModel.data.dynamicNumber.phoneNumberResp[0].local;

                phone = await DecryptAsync(encrypted, uuid);
            }

            var make = doc.DocumentNode.SelectSingleNode("//p[text()='Marca']/../following-sibling::p")?.InnerText
                .Trim();
            var model = doc.DocumentNode.SelectSingleNode("//p[text()='Modelo']/../following-sibling::p")?.InnerText
                .Trim();
            var color = doc.DocumentNode.SelectSingleNode("//p[text()='Cor']/../following-sibling::p").InnerText.Trim();
            var kilometers = doc.DocumentNode.SelectSingleNode("//p[text()='Quilómetros']/following-sibling::p")
                .InnerText.Trim();
            var dealer = doc.DocumentNode?.SelectSingleNode("//div[@data-testid='seller-header']/div/div/p")?.InnerText
                .Trim();
            var price = doc.DocumentNode.SelectSingleNode("//h3[@class='offer-price__number']").InnerText.Trim();
            // var registrationDate = int.Parse(doc.DocumentNode
            //     .SelectSingleNode("//p[contains(text(),'No Standvirtual desde')]")
            //     .InnerText.Trim().Replace("No Standvirtual desde ", ""));
            var registrationDateTxt = doc.DocumentNode.SelectSingleNode("//h1/following-sibling::p").InnerText.Trim();
            var registrationDate= ExtractYear(registrationDateTxt);
            var advertisingDateText =
                doc.DocumentNode.SelectSingleNode("//p[contains(text(),'ID')]/../../preceding-sibling::p").InnerText
                    .Trim();
            var array = advertisingDateText.Split(" às ");
            advertisingDateText = array[0];
            var culture = new CultureInfo("pt-PT");
            var advertisingDate = DateTime.Parse(advertisingDateText, culture);
            car = new Car
            {
                DealerName = dealer,
                Model = model,
                Color = color,
                Kilometre = kilometers,
                RegistartionDate = registrationDate,
                Price = $"{price} Euro",
                Make = make,
                Phone = phone,
                Url = carUrl,
                AdvertisingDate = advertisingDate
            };
        }
        catch (Exception e)
        {
            //
        }

        return car;
    }

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in normalized)
        {
            var category = Char.GetUnicodeCategory(ch);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static async Task<string> DecryptAsync(string encryptedData, string sellerUuid)
    {
        // Offload CPU-heavy cryptographic operations (like PBKDF2) to a background thread
        return await Task.Run(() => Decrypt(encryptedData, sellerUuid));
    }

    private static string Decrypt(string? encryptedData, string? sellerUuid)
    {
        encryptedData = encryptedData?.Trim().Trim('"', '\'');
        sellerUuid = sellerUuid?.Trim().Trim('"', '\'');
        // 1. Split the payload into [ciphertext+tag, version, iv]
        string[] parts = encryptedData.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Invalid encrypted data format.");

        // 2. Decode the Base64 components
        byte[] encryptedBytes = DecodeBase64(parts[0]); // Ciphertext + 16-byte Tag
        string version = parts[1];
        byte[] iv = DecodeBase64(parts[2]); // The Nonce (likely 16 bytes, causing the .NET error)

        // 3. Derive the AES-GCM key
        byte[] key = DeriveKey(sellerUuid, version);

        // 4. Use BouncyCastle to decrypt (Bypasses the strict 12-byte Windows limitation)
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key), 128, iv); // 128-bit MAC/Tag size

        cipher.Init(false, parameters);

        // 5. Decrypt
        byte[] plainText = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
        int len = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainText, 0);
        cipher.DoFinal(plainText, len);

        // 6. Return decoded UTF-8 string
        return Encoding.UTF8.GetString(plainText);
    }

    [Obsolete("Obsolete")]
    private static byte[] DeriveKey(string sellerUuid, string version)
    {
        // JS Function 'l': SHA-256 the sellerUuid
        byte[] uuidBytes = Encoding.UTF8.GetBytes(sellerUuid);

        // Use SHA256.HashData for .NET 5+. For older frameworks, use SHA256.Create().ComputeHash()
        byte[] hash = SHA256.HashData(uuidBytes);

        // Take the first 16 bytes of the hash and convert to lowercase hex string
        byte[] truncatedHash = hash.Take(16).ToArray();
        string hexString = BitConverter.ToString(truncatedHash).Replace("-", "").ToLowerInvariant();

        // JS Function 'c': PBKDF2 Key Derivation
        byte[] password = Encoding.UTF8.GetBytes(hexString);
        byte[] salt = "d2905222-d0c5-4ec5-bfcf-e9c29041de3c"u8.ToArray();

        // Iterations logic: 10 if version isn't empty/null and isn't '0', else 10000
        int iterations = (!string.IsNullOrEmpty(version) && version != "0") ? 10 : 10000;

        using var rfc2898 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return rfc2898.GetBytes(32); // AES-256 needs a 32-byte key length
    }

    private static byte[] DecodeBase64(string base64)
    {
        // JavaScript's atob is forgiving, but C# is strict about Base64 padding.
        // This adds the missing '=' padding characters if needed.
        int mod4 = base64.Length % 4;
        if (mod4 > 0)
        {
            base64 += new string('=', 4 - mod4);
        }

        return Convert.FromBase64String(base64);
    }

    private static int? ExtractYear(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var match = YearRegex.Match(input);
        return match.Success ? int.Parse(match.Value) : null;
    }}