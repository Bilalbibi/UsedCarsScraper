namespace UsedCarsScraper.Models
{
    public class Car
    {
        public string Url { get; set; }
        public string? Phone { get; set; }
        public string? DealerName { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        public string? Price { get; set; }
        public string Kilometre { get; set; }
        public int? RegistartionDate { get; set; }

        public DateTime? AdvertisingDate { get; set; }
    }
}
