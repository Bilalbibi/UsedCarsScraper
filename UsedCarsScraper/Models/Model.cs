namespace UsedCarsScraper.Models
{
    public class Model
    {
        public string? Name { get; set; }
        public string SearchKey { get; set; }
        public string? Id { get; set; }
        public List<SubModel> SubModels { get; set; } = [];

        public override string? ToString() => Name;

        public override bool Equals(object? obj)
        {
            // Handle null and reference equality
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null) return false;
        
            // Safe type check using pattern matching
            if (obj is Model other)
            {
                // Compare by SearchKey (your unique identifier)
                return SearchKey?.Equals(other.SearchKey, StringComparison.OrdinalIgnoreCase) ?? false;
            }
        
            return false;
        }

        protected bool Equals(Model other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) 
                   && string.Equals(SearchKey, other.SearchKey, StringComparison.OrdinalIgnoreCase) 
                   && string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            // Use ordinal ignore case for consistency with Equals
            return HashCode.Combine(
                SearchKey?.ToLowerInvariant(),
                Name?.ToLowerInvariant(),
                Id?.ToLowerInvariant()
            );
        }
    }
}
