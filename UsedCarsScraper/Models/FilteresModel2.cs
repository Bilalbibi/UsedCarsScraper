namespace UsedCarsScraper.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Component
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string parentID { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public object tag { get; set; }
        public object placeholder { get; set; }
        public DisplayConfig displayConfig { get; set; }
    }

    public class Condition
    {
        public string __typename { get; set; }
        public string filterId { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class DisplayConfig
    {
        public string __typename { get; set; }
        public string renderAs { get; set; }
        public bool? hasMultiple { get; set; }
        public string inputMode { get; set; }
        public string suffix { get; set; }
    }

    public class Filters
    {
        public List<Component> components { get; set; }
        public List<State> states { get; set; }
        public string __typename { get; set; }
    }

    public class FilteresModel2
    {
        public string __typename { get; set; }
        public SortOptions sortOptions { get; set; }
        public Filters filters { get; set; }
    }

    public class SortOptions
    {
        public List<State> states { get; set; }
        public string __typename { get; set; }
    }

    public class State
    {
        public List<Value> values { get; set; }
        public List<string> defaultSelectedValues { get; set; }
        public string __typename { get; set; }
        public string filterId { get; set; }
        public List<Condition> conditions { get; set; }
    }

    public class Value
    {
        public List<Value> values { get; set; }
        public string __typename { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public int? counter { get; set; }
    }

