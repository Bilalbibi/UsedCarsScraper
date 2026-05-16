namespace UsedCarsScraper.StandVirtualModels
{
    public class StandVirtualConfig
    {
        public List<StandVirtualMake> Makes { get; set; } = [];
        public List<string>? Dates { get; set; }
        public Dictionary<string,string> FuelTypes { get; set; }
    }
}