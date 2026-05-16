using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UsedCarsScraper.Extensions;
using UsedCarsScraper.GeneralModels;
using UsedCarsScraper.StandVirtualModels;

namespace UsedCarsScraper.Services;

public class StandVirtualConfigsScraper
{
    private List<Car> _cars = [];
    private StandVirtualConfig _standVirtualConfig = new();

    private HttpClient _client =
        new(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.All, })
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

    public List<StandVirtualInputModel> InputModels = [];

    public async Task StandVirtualConfigurationsScraper()
    {
        var json = await File.ReadAllTextAsync("StandVirtual config2 file");
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var filters = root.GetProperty("filters");
        
         var makes = ExtractFilterValues(filters, "filter_enum_make")
            .Select(v => new StandVirtualMake { Id = v.Id, Name = v.Name, SearchKey = v.Id?.ToLowerInvariant() })
            .ToList();

        // 2. Extract model states (grouped by make condition)
        var modelStates = filters.GetProperty("states")
            .EnumerateArray()
            .Where(s => s.GetProperty("filterId").GetString() == "filter_enum_model")
            .ToList();

        // 3. Extract submodel states (grouped by make+model conditions)
        var submodelStates = filters.GetProperty("states")
            .EnumerateArray()
            .Where(s => s.GetProperty("filterId").GetString() == "filter_enum_engine_code")
            .ToList();

        // 4. Build hierarchy: Make → Models → SubModels
        foreach (var make in makes)
        {
            // Get models for this make
            var makeModels = modelStates
                .Where(ms => ConditionMatches(ms, "filter_enum_make", make.Id))
                .SelectMany(ms => ExtractValuesFromState(ms))
                .DistinctBy(m => m.Id)
                .Select(v => new StandVirtualModel 
                { 
                    Id = v.Id, 
                    Name = v.Name, 
                    SearchKey = $"{make.SearchKey}:{v.Id}".ToLowerInvariant() 
                })
                .ToList();

            foreach (var model in makeModels)
            {
                // Get submodels for this make+model combination
                var subModels = submodelStates
                    .Where(ss => ConditionMatches(ss, "filter_enum_make", make.Id) && 
                                ConditionMatches(ss, "filter_enum_model", model.Id))
                    .SelectMany(ss => ExtractValuesFromState(ss))
                    .DistinctBy(s => s.Id)
                    .Select(v => new StandVirtualSubModel 
                    { 
                        Id = v.Id, 
                        Name = v.Name, 
                        SearchKey = $"{make.SearchKey}:{model.SearchKey}:{v.Id}".ToLowerInvariant(),
                        Counter = v.Counter 
                    })
                    .ToList();
                
                model.SubModels.AddRange(subModels);
            }
            
            make.Models.AddRange(makeModels);
        }

        var fuelTypes = ExtractFilterValues(filters, "filter_enum_fuel_type").Select(v => new SubValue() { Id = v.Id, name = v.Name }).ToDictionary(value => value.Id, value => value.name);

        var config = new StandVirtualConfig
        {
            Makes = makes,
            Dates = [],
            FuelTypes = fuelTypes
        };
        try
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ObjectCreationHandling = ObjectCreationHandling.Reuse,
                NullValueHandling = NullValueHandling.Ignore
            };
            var jsonConfig = JsonConvert.SerializeObject(config, settings);
            await File.WriteAllTextAsync("StandVirtual config file", jsonConfig);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    private static IEnumerable<(string Id, string Name, int? Counter)> ExtractFilterValues(JsonElement filters, string filterId)
    {
        var states = filters.GetProperty("states");
    
        foreach (var state in states.EnumerateArray())
        {
            if (state.GetProperty("filterId").GetString() == filterId)
            {
                return ExtractValuesFromState(state);
            }
        }
    
        return Enumerable.Empty<(string, string, int?)>();
    }
    private static bool ConditionMatches(JsonElement state, string targetFilterId, string targetValue)
    {
        if (!state.TryGetProperty("conditions", out var conditions))
            return false;
            
        return conditions.EnumerateArray()
            .Any(c => c.GetProperty("filterId").GetString() == targetFilterId && 
                      c.GetProperty("value").GetString()?.ToLowerInvariant() == targetValue?.ToLowerInvariant());
    }
    private static IEnumerable<(string Id, string Name, int? Counter)> ExtractValuesFromState(JsonElement state)
    {
        if (!state.TryGetProperty("values", out var valuesArray))
            yield break;
            
        foreach (var group in valuesArray.EnumerateArray())
        {
            if (!group.TryGetProperty("values", out var innerValues))
                continue;
                
            foreach (var item in innerValues.EnumerateArray())
            {
                var id = item.GetProperty("id").GetString();
                var name = item.GetProperty("name").GetString();
                var counter = item.TryGetProperty("counter", out var c) && c.ValueKind != JsonValueKind.Null 
                    ? c.GetInt32() 
                    : (int?)null;
                    
                if (!string.IsNullOrEmpty(id))
                    yield return (id, name, counter);
            }
        }
    }
}