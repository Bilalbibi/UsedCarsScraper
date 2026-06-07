using System.Text.Json;
using Microsoft.Playwright;

namespace UsedCarsScraper.Services;

public sealed class LeBonCoinLoginService : IAsyncDisposable
{
    private const string HomeUrl = "https://www.leboncoin.fr/";
    private const string AuthenticatorLoginEndpoint = "/api/authenticator/v2/users/login";
    private const string AuthorizerTokenEndpoint = "/api/authorizer/v2/token";

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    public async Task<LeBonCoinLoginResult> LoginAsync(
        string email,
        string password,
        IProgress<string>? progress = null,
        bool headless = false,
        bool keepBrowserAlive = false,
        int timeoutMs = 90_000,
        int challengeTimeoutMs = 300_000,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required.", nameof(password));
        }

        string? authenticatorLoginResponse = null;
        string? authorizerTokenResponse = null;

        do
        {
           // await CloseBrowserAsync();

           try
           {
               _playwright = await Playwright.CreateAsync();
               _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
               {
                   Headless = headless,
                   Channel = "chrome"
               });

               _context = await _browser.NewContextAsync(new BrowserNewContextOptions
               {
                   Locale = "fr-FR",
                   ViewportSize = new ViewportSize
                   {
                       Width = 1366,
                       Height = 900
                   }
               });

               _page = await _context.NewPageAsync();
               var context = _context;
               var page = _page;
               page.SetDefaultTimeout(timeoutMs);

               page.Response += async (_, response) =>
               {
                   if (response.Url.Contains(AuthenticatorLoginEndpoint, StringComparison.OrdinalIgnoreCase))
                   {
                       authenticatorLoginResponse = await ReadResponseBodyAsync(response);
                       progress?.Report($"Captured {AuthenticatorLoginEndpoint}");
                       // PrintResponse(AuthenticatorLoginEndpoint, response, authenticatorLoginResponse);
                       return;
                   }

                   if (response.Url.Contains(AuthorizerTokenEndpoint, StringComparison.OrdinalIgnoreCase))
                   {
                       authorizerTokenResponse = await ReadResponseBodyAsync(response);
                       progress?.Report($"Captured {AuthorizerTokenEndpoint}");
                       // PrintResponse(AuthorizerTokenEndpoint, response, authorizerTokenResponse);
                   }
               };

               progress?.Report("Opening leboncoin home page...");
               await page.GotoAsync(HomeUrl, new PageGotoOptions
               {
                   WaitUntil = WaitUntilState.Load,
                   Timeout = timeoutMs
               });

               await page.WaitForTimeoutAsync(10_000);
               await Task.Delay(20000, cancellationToken);
               var isSolved = await WaitForHomeLoginEntryAsync(page, challengeTimeoutMs, progress, cancellationToken);
               if (!isSolved)
               {
                   await page.CloseAsync();
                   await Task.Delay(2000, cancellationToken);
                   continue;
               }

               await ClickFirstVisibleAsync(page, 5_000,
               [
                   "button:has-text(\"Tout accepter\")",
                   "button:has-text(\"Accepter\")",
                   "button:has-text(\"J'accepte\")",
                   "button:has-text(\"J’accepte\")"
               ]);

               progress?.Report("Clicking Se connecter...");
               var clickedLogin = await ClickFirstVisibleAsync(page, 10_000,
               [
                   "text=Se connecter",
                   "button:has-text(\"Se connecter\")",
                   "a:has-text(\"Se connecter\")",
                   "button:has-text(\"Connexion\")",
                   "a:has-text(\"Connexion\")",
                   "button:has-text(\"Connectez-vous\")",
                   "a:has-text(\"Connectez-vous\")",
                   "[data-test-id*=\"login\" i]",
                   "[href*=\"login\" i]"
               ]);

               if (!clickedLogin)
               {
                   await ClickFirstVisibleAsync(page, 10_000,
                   [
                       "text=Compte",
                       "button:has-text(\"Compte\")",
                       "a:has-text(\"Compte\")",
                       "button[aria-label*=\"compte\" i]",
                       "a[aria-label*=\"compte\" i]",
                       "[href*=\"compte\" i]"
                   ], required: true);

                   await ClickFirstVisibleAsync(page, timeoutMs,
                   [
                       "text=Se connecter",
                       "button:has-text(\"Se connecter\")",
                       "a:has-text(\"Se connecter\")",
                       "button:has-text(\"Connexion\")",
                       "a:has-text(\"Connexion\")",
                       "button:has-text(\"Connectez-vous\")",
                       "a:has-text(\"Connectez-vous\")",
                       "[data-test-id*=\"login\" i]",
                       "[href*=\"login\" i]"
                   ], required: true);
               }

               progress?.Report("Submitting email...");
               await Task.Delay(5000, cancellationToken);
               await FillVisibleSelectorAsync(page, "xpath=//input[@id='email']", email, timeoutMs);

               await ClickFirstVisibleAsync(page, 5_000,
               [
                   "button[type=\"submit\"]",
                   "button:has-text(\"Continuer\")",
                   "button:has-text(\"Se connecter\")",
                   "button:has-text(\"Connexion\")"
               ]);
               await Task.Delay(5000, cancellationToken);
               progress?.Report("Submitting password...");
               await FillVisibleSelectorAsync(page, "xpath=//input[@id='password']", password, timeoutMs);

               await ClickFirstVisibleAsync(page, timeoutMs,
               [
                   "button[type=\"submit\"]",
                   "button:has-text(\"Se connecter\")",
                   "button:has-text(\"Connexion\")",
                   "button:has-text(\"Continuer\")"
               ], required: true);

               await WaitForTokenResponseAsync(
                   () => authorizerTokenResponse is not null,
                   timeoutMs,
                   cancellationToken);

               var datadomeCookie = await GetCookieValueAsync(context, "datadome");
               await ClickFirstVisibleAsync(page, 10_000,
               [
                   "button:has-text(\"Tout accepter\")",
                   "button:has-text(\"Accepter\")",
                   "button:has-text(\"J'accepte\")",
                   "button:has-text(\"J’accepte\")"
               ]);
               await MinimizeChromeWindowAsync(page);

               return new LeBonCoinLoginResult(
                   authenticatorLoginResponse,
                   authorizerTokenResponse,
                   datadomeCookie);
           }
           catch (Exception e)
           {
               await CloseBrowserAsync();
               await Task.Delay(20000, cancellationToken);
               _playwright = null;
           }
        } while (true);
    }

    private static async Task<string> ReadResponseBodyAsync(IResponse response)
    {
        try
        {
            return await response.TextAsync();
        }
        catch (Exception ex)
        {
            return $"<failed to read response body: {ex.Message}>";
        }
    }

    private static async Task<bool> WaitForHomeLoginEntryAsync(
        IPage page,
        int challengeTimeoutMs,
        IProgress<string>? progress,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var iframeElement = await page.QuerySelectorAsync("xpath=//iframe[@title='DataDome CAPTCHA']");
        try
        {
            if (iframeElement != null)
            {
                var frame = await iframeElement?.ContentFrameAsync()!;

                // Optional: Wait for frame to be ready
                await frame?.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await frame.WaitForSelectorAsync("xpath=//button[@id='captcha__audio__button']",
                    new FrameWaitForSelectorOptions { State = WaitForSelectorState.Visible });

                //Console.ReadLine();
                var handle = frame.Locator("xpath=//div[@class='slider']");
                var track = frame.Locator("xpath=//div[@class='sliderTarget']");


                // 3. Wait for the elements inside the iframe to load
                await handle.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

                // 4. Extract absolute screen coordinates for calculations
                var handleBox = await handle.BoundingBoxAsync();
                var trackBox = await track.BoundingBoxAsync();

                if (handleBox != null && trackBox != null)
                {
                    // Center of your handle element to click on
                    var startX = handleBox.X + (handleBox.Width / 2);
                    var startY = handleBox.Y + (handleBox.Height / 2);

                    // Total slider track movement width minus the width of the piece itself
                    var totalSlideDistance = startX - startY;

                    // 5. Begin mouse manipulation
                    await page.Mouse.MoveAsync(startX, startY);
                    await page.Mouse.DownAsync();

                    var currentX = startX;
                    var rnd = new Random();
                    var steps = rnd.Next(17, 36);
                    var totalSteps = steps; // Increased steps for smoother, less automated look
                    var random = new Random();

                    for (int i = 1; i <= totalSteps; i++)
                    {
                        // Break down the total width into small chunks and add human jitter/variance
                        var jitter = random.Next(-2, 3);
                        currentX += (totalSlideDistance / totalSteps) + jitter;

                        await page.Mouse.MoveAsync(currentX, startY);
                        await Task.Delay(random.Next(10, 45)); // Micro delays to simulate drag friction
                    }

                    // Move precisely to the final edge offset and release
                    await page.Mouse.MoveAsync(startX + totalSlideDistance, startY);
                    await page.Mouse.UpAsync();

                    Console.WriteLine("Slider drag simulation completed.");
                }
                else
                {
                    Console.WriteLine(
                        "Could not calculate bounding boxes. Check if the iframe container is hidden.");
                }

                await Task.Delay(10000, cancellationToken);
                var restrictionText = page.Locator("xpath=//span[text()='Vacances']");

                await restrictionText.WaitForAsync(new()
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 10000
                });
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    private static async Task WaitForTokenResponseAsync(
        Func<bool> hasTokenResponse,
        int timeoutMs,
        CancellationToken cancellationToken)
    {
        var deadline = DateTimeOffset.UtcNow.AddMilliseconds(timeoutMs);
        while (DateTimeOffset.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (hasTokenResponse())
            {
                return;
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException($"Timed out waiting for {AuthorizerTokenEndpoint}.");
    }

    private static async Task<string?> GetCookieValueAsync(IBrowserContext context, string cookieName)
    {
        var cookies = await context.CookiesAsync();
        return cookies.FirstOrDefault(cookie =>
            cookie.Name.Equals(cookieName, StringComparison.OrdinalIgnoreCase))?.Value;
    }

    private static async Task MinimizeChromeWindowAsync(IPage page)
    {
        try
        {
            var session = await page.Context.NewCDPSessionAsync(page);
            var window = await session.SendAsync("Browser.getWindowForTarget");
            if (!window.HasValue ||
                !window.Value.TryGetProperty("windowId", out var windowIdElement) ||
                !windowIdElement.TryGetInt32(out var windowId))
            {
                return;
            }

            await session.SendAsync("Browser.setWindowBounds", new Dictionary<string, object>
            {
                ["windowId"] = windowId,
                ["bounds"] = new Dictionary<string, object>
                {
                    ["windowState"] = "minimized"
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not minimize Chrome window: {ex.Message}");
        }
    }

    private static async Task FillVisibleSelectorAsync(IPage page, string selector, string value, float timeoutMs)
    {
        var locator = page.Locator(selector).First;
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs
        });
        await locator.ClickAsync(new LocatorClickOptions
        {
            Timeout = timeoutMs
        });
        await locator.FillAsync(value, new LocatorFillOptions
        {
            Timeout = timeoutMs
        });
    }

    private static async Task<bool> HasAnySelectorAsync(IPage page, IReadOnlyList<string> selectors)
    {
        foreach (var selector in selectors)
        {
            try
            {
                var locator = page.Locator(selector).First;
                if (await locator.CountAsync() > 0 && await locator.IsVisibleAsync())
                {
                    return true;
                }
            }
            catch (TimeoutException)
            {
            }
            catch (PlaywrightException)
            {
            }
        }

        return false;
    }

    private static async Task<bool> ClickFirstVisibleAsync(
        IPage page,
        float timeoutMs,
        IReadOnlyList<string> selectors,
        bool required = false)
    {
        foreach (var selector in selectors)
        {
            var locator = page.Locator(selector).First;
            try
            {
                if (await locator.CountAsync() > 0 && await locator.IsVisibleAsync())
                {
                    await locator.ClickAsync(new LocatorClickOptions
                    {
                        Timeout = timeoutMs
                    });
                    return true;
                }
            }
            catch (TimeoutException)
            {
            }
            catch (PlaywrightException)
            {
            }
        }

        if (required)
        {
            throw new InvalidOperationException(
                $"Could not click any selector: {string.Join(", ", selectors)}");
        }

        return false;
    }

    private static void PrintResponse(string endpoint, IResponse response, string body)
    {
        Console.WriteLine();
        Console.WriteLine($"===== {endpoint} =====");
        Console.WriteLine($"URL: {response.Url}");
        Console.WriteLine($"Status: {response.Status} {response.StatusText}");
        Console.WriteLine("Headers:");
        Console.WriteLine(JsonSerializer.Serialize(response.Headers, CreateJsonOptions()));
        Console.WriteLine("Body:");
        Console.WriteLine(FormatBody(body));
    }

    private static string FormatBody(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            return JsonSerializer.Serialize(document.RootElement, CreateJsonOptions());
        }
        catch (JsonException)
        {
            return body;
        }
    }

    private static JsonSerializerOptions CreateJsonOptions() => new()
    {
        WriteIndented = true
    };

    private async Task CloseBrowserAsync()
    {
        if (_page is not null)
        {
            await _page.CloseAsync();
            _page = null;
        }

        if (_context is not null)
        {
            await _context.CloseAsync();
            _context = null;
        }

        if (_browser is not null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;
    }

    public async ValueTask DisposeAsync()
    {
        await CloseBrowserAsync();
    }
    public async Task LogOut()
    {
        try
        {
            var timeoutMs = 90_000;
            await _page.GotoAsync("https://www.leboncoin.fr/account/private/home", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Load,
                Timeout = timeoutMs
            });
            await ClickFirstVisibleAsync(_page, 5_000,
            [
                "button[type=\"button\"]",
                "button:has-text(\"Me déconnecter\")",
                "button:has-text(\"Se connecter\")",
                "button:has-text(\"Connexion\")"
            ]);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        await Task.Delay(10000);
        await CloseBrowserAsync();
    }
}
public sealed record LeBonCoinLoginResult(
    string? AuthenticatorLoginResponse,
    string? AuthorizerTokenResponse,
    string? DatadomeCookie);
