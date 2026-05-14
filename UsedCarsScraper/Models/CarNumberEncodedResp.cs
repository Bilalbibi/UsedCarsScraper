namespace UsedCarsScraper.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Data
{
    public DynamicNumber dynamicNumber { get; set; }
}

public class DynamicNumber
{
    public string __typename { get; set; }
    public List<PhoneNumberResp> phoneNumberResp { get; set; }
}

public class PhoneNumberResp
{
    public string e164 { get; set; }
    public string local { get; set; }
    public string __typename { get; set; }
}

public class CarNumberEncodedResp
{
    public Data data { get; set; }
}