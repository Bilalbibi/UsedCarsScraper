using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using UsedCarsScraper.LeBonCoinModels;

namespace UsedCarsScraper.Services;

public class LeBonCoinCarDataExtractor
{
    public async Task ExtractCarData()
    {
        using var doc = JsonDocument.Parse(await File.ReadAllTextAsync("leboncoin config"));
        var features = doc.RootElement.GetProperty("features");
        var brandsDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var cars = new List<LeBonCoinCarBrand>();

        var fuels = new List<Fuel>();
        
        if (features.TryGetProperty("fuel", out var fuelFeature) &&
            fuelFeature.TryGetProperty("values", out var fuelValues) &&
            fuelValues.TryGetProperty("simpleData", out var fuelData))
        {
            foreach (var item in fuelData.EnumerateArray())
            {
                var val = item.GetProperty("value").GetString();
                var lbl = item.GetProperty("label").GetString();

                // Filter out "Autre" (Value 5)
                if (!string.IsNullOrEmpty(val) && 
                    !lbl.Equals("Autre", StringComparison.OrdinalIgnoreCase))
                {
                    fuels.Add(new Fuel
                    {
                        Name = lbl,
                        Value = val
                    });
                }
            }
        }
        
        // 1️⃣ Index all valid brands for fast lookup
        if (features.TryGetProperty("u_car_brand", out var brandProp) &&
            brandProp.TryGetProperty("values", out var brandValues) &&
            brandValues.TryGetProperty("groupedData", out var groupedData))
        {
            foreach (var group in groupedData.EnumerateArray())
            {
                if (group.TryGetProperty("list", out var list))
                {
                    foreach (var item in list.EnumerateArray())
                    {
                        var val = item.GetProperty("value").GetString();
                        var label = item.GetProperty("label").GetString();
                        if (!string.IsNullOrEmpty(val) && !val.Equals("AUTRE", StringComparison.OrdinalIgnoreCase))
                        {
                            brandsDict[val] = label;
                        }
                    }
                }
            }
        }

        // 2️⃣ Parse dynamic model keys and map them to brands
        foreach (var feature in features.EnumerateObject())
        {
            if (!feature.Name.StartsWith("u_car_model_", StringComparison.Ordinal)) continue;

            var brandKey = feature.Name.Substring("u_car_model_".Length);
            if (brandKey.Equals("autre", StringComparison.OrdinalIgnoreCase)) continue;
            
            var lookupKey = brandKey.Replace('_', ' ');

            if (!brandsDict.TryGetValue(lookupKey, out var brandLabel)) continue;

            var modelsList = new List<LeBonCoinCarModel>();
            if (feature.Value.TryGetProperty("values", out var modelValues) &&
                modelValues.TryGetProperty("simpleData", out var simpleData))
            {
                foreach (var item in simpleData.EnumerateArray())
                {
                    var val = item.GetProperty("value").GetString();
                    var label = item.GetProperty("label").GetString();

                    // Skip fallback "Autre" entries
                    if (!string.IsNullOrEmpty(val) && !val.EndsWith("Autre", StringComparison.OrdinalIgnoreCase))
                    {
                        modelsList.Add(new LeBonCoinCarModel
                        {
                            Name = label,
                            Value = val
                        });
                    }
                }
            }

            cars.Add(new LeBonCoinCarBrand
            {
                Name = brandLabel.ToUpper(),
                Value = lookupKey,
                Models = modelsList
            });
        }

        var leBonCoinConfig = new LeBonCoinConfig
        {
            BonCoinCarBrands = cars,
            Fuels = fuels
        };
        
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Reuse,
            NullValueHandling = NullValueHandling.Ignore
        };
        var jsonConfig = JsonConvert.SerializeObject(leBonCoinConfig, settings);
        await File.WriteAllTextAsync("leboncoin config file", jsonConfig);
    }
}
