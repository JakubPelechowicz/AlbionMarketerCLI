namespace AlbionMarketer.Models;

public record ResourceCityModel
{
    public string City { get; set; }
    public string SellPrice { get; set; }
    public string BuyPrice { get; set; }
    public string SellPriceLastUpdate { get; set; }
    public string BuyPriceLastUpdate { get; set; }
    public string? Note { get; set; }
}