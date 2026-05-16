namespace UsedCarsScraper.LeBonCoinModels;

public class LeBonCoinCarBrand
{
    public string? Name { get; set; }
    public List<LeBonCoinCarModel> Models { get; set; } = [];
}
