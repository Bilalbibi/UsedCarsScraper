namespace UsedCarsScraper.Models;

public class SubModel
{
    public string? Name { get; set; }
    public string SearchKey { get; set; }  // e.g., "mercedes-benz:classe-sl:sl-350"
    public string? Id { get; set; }         // e.g., "sl-350"
    public int? Counter { get; set; }       // Available ads count

    public override string? ToString() => Name;

    public override bool Equals(object obj)
    {
        return SearchKey?.Equals(((SubModel)obj)?.SearchKey) ?? false;
    }

    protected bool Equals(SubModel other)
    {
        return Name == other.Name && SearchKey == other.SearchKey && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, SearchKey, Id);
    }
}