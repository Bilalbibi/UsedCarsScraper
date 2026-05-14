using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UsedCarsScraper.Extensions;
using UsedCarsScraper.Models;

namespace UsedCarsScraper.Services;

public class ConfigsScraper
{
    private List<Car> _cars = [];
    private Config _config = new();

    private HttpClient _client =
        new(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.All, })
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

    public List<InputModel> InputModels = [];

    public async Task StandVirtualConfigurationsScraper()
    {
        var json = await File.ReadAllTextAsync("StandVirtual config2 file");
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var filters = root.GetProperty("filters");
        
         var makes = ExtractFilterValues(filters, "filter_enum_make")
            .Select(v => new Make { Id = v.Id, Name = v.Name, SearchKey = v.Id?.ToLowerInvariant() })
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
                .Select(v => new Model 
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
                    .Select(v => new SubModel 
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
        var config = new Config
        {
            Makes = makes,
            Dates = []
        };
        try
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                // Prevents Newtonsoft from trying to replace read-only lists
                ObjectCreationHandling = ObjectCreationHandling.Reuse,
                // Ignores null values for cleaner JSON
                NullValueHandling = NullValueHandling.Ignore
            };
            var jsonConfig = JsonConvert.SerializeObject(config, settings);
            await File.WriteAllTextAsync("StandVirtual config file", jsonConfig);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        #region old filter model for conf

        // var doc = (await _client.Get("https://www.standvirtual.com/",5,[new KeyValuePair<string, string>("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36")])).Text.ToDoc();
        // var json = doc.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']").InnerText.Trim();
        //await File.WriteAllTextAsync("StandVirtual config file", json);
        // var json = await File.ReadAllTextAsync("StandVirtual config file");
        //
        // try
        // {
        //     var filers = JsonConvert.DeserializeObject<FilteresModel>(json);
        //     var makes = new List<Make>();
        //     var dates = new List<string>();
        //     var makeModelFilers=filers?.props.pageProps.screenComponentsFilters.states;
        //     var makesValue=makeModelFilers.FirstOrDefault(x=>x.filterId=="filter_enum_make").values[0].values;
        //     var modelValues=makeModelFilers.Where(x=>x.filterId=="filter_enum_model").ToList();
        //     var dateValues=makeModelFilers.FirstOrDefault(x=>x.filterId=="filter_float_first_registration_year:from").values[0].values;
        //     dateValues.Reverse();
        //     foreach (var makeFilter in makesValue)  
        //     {
        //         var make = new Make
        //         {
        //             Name = makeFilter.name,
        //             Id = makeFilter.Id
        //         };
        //         var models= modelValues.FirstOrDefault(x=>x.conditions[0].value.Equals(makeFilter.Id?.ToLower(), StringComparison.CurrentCultureIgnoreCase))?.values[0].values;
        //         foreach (var model in models)
        //         {
        //             make.Models.Add(new Model
        //             {
        //                 Name = model.name,
        //                 Id = model.Id
        //             });
        //         }
        //         makes.Add(make);
        //     }
        //
        //     foreach (var dateValue in dateValues)
        //     {
        //         dates.Add(dateValue.name);
        //     }
        //
        //     var config = new Config
        //     {
        //         Makes = makes
        //         , Dates = dates
        //     };
        //     var jsonConfig = JsonConvert.SerializeObject(config, Formatting.Indented);
        //     await File.WriteAllTextAsync("StandVirtual config file", jsonConfig);
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        // }
        // _config.Makes = makes;
        // _config.Dates = dates;
        //await File.WriteAllTextAsync("StandVirtual config file", JsonConvert.SerializeObject(_config, Formatting.Indented));

        #endregion
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