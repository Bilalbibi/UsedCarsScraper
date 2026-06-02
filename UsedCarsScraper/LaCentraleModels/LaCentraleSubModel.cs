namespace UsedCarsScraper.LaCentraleModels;

public class LaCentraleSubModel
{
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? SearchKey { get; set; }
    public int? Counter { get; set; }

    public override string? ToString() => Name;
}
