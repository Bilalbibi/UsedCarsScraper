namespace UsedCarsScraper.StandVirtualModels
{
    public class StandVirtualMake
    {
        public string? SearchKey { get; set; }
        public string? Name { get; set; }
        public string Id { get; set; }
        public List<StandVirtualModel> Models { get; } = [];
        public override string ToString() => Name;
    }
}
