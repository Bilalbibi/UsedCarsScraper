using Microsoft.Playwright;

namespace UsedCarsScraper.Services;

public class DataDomeRaper
{
    public async Task<(string dataDome, string SecureInstall, string route)> DataDomeCookieCatcher(string url)
    {
        var dic = new Dictionary<string, string>();
        var route = "";
        var tries = 0;
        try
        {
            do
            {
                var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    //Args = ["--window-size=50,50", "--window-position=-2000,0"]
                    //Args = ["--headless=new", "--disable-blink-features=AutomationControlled"]
                });
                var contextOptions = new BrowserNewContextOptions
                {
                    Proxy = new Proxy
                    {
                        Server = "84.247.60.125:6095", // e.g., http://example.com
                        Username = "wxxedufq",
                        Password = "xt007td5f6dc"
                    }
                };
                var context = await browser.NewContextAsync();
                // await context.AddInitScriptAsync(
                //     @"Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
                var page = await context.NewPageAsync();
                await page.GotoAsync(url);
                await Task.Delay(5000);
                var iframeElement = await page.QuerySelectorAsync("xpath=//iframe[@title='DataDome CAPTCHA']");
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

                    await Task.Delay(10000);
                    var restrictionText = page.Locator("xpath=//span[text()='Vacances']");
                    var isFound = false;
                    try
                    {
                        await restrictionText.WaitForAsync(new()
                        {
                            State = WaitForSelectorState.Visible,
                            Timeout = 10000
                        });

                        Console.WriteLine("Block page detected! Access is restricted.");
                    }
                    catch (TimeoutException)
                    {
                        isFound = true;
                    }

                    if (isFound)
                    {
                        if (tries == 3)
                        {
                            return ("not found", "not found", route);
                        }

                        tries++;
                        await page.CloseAsync();
                        await Task.Delay(2000);
                        continue;
                    }
                    //p[contains(text(),'restricted')]
                }

                //Console.ReadLine();
                var cookies = await page.Context.CookiesAsync();
                foreach (var cookie in cookies)
                {
                    Console.WriteLine($"Name: {cookie.Name}, Value: {cookie.Value}");
                    dic.Add(cookie.Name, cookie.Value);
                }

                var elements = await page.Locator("xpath=//script[@defer][last()]").GetAttributeAsync("src");
                var gQl = elements?.Replace("/_next/static/", "");
                if (gQl != null)
                {
                    var x = gQl.LastIndexOf('/');
                    route = gQl[..x];
                }

                await page.CloseAsync();
                break;
            } while (true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        var dataDome = dic["datadome"];
        var secureInstall = dic["__Secure-Install"];
        return (dataDome, secureInstall, route);
    }
}
