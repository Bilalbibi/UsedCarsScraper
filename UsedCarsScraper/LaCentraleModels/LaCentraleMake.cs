namespace UsedCarsScraper.LaCentraleModels;

public class LaCentraleMake
{
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? SearchKey { get; set; }
    public List<LaCentraleModel> Models { get; set; } = [];

    public override string? ToString() => Name;
}
