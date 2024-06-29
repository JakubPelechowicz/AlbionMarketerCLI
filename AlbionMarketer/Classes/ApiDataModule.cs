using AlbionMarketer.Models;
using Newtonsoft.Json.Linq;

namespace AlbionMarketer.Classes;

public static class ApiDataModule
{
    public static async Task<List<ResourceCityModel>> GetItemPrices(string itemId)
    {
        var jsonArray = await RequestApiForSingleResource(itemId);

        var resourceCityModels = new List<ResourceCityModel>();

        foreach (var token in jsonArray)
        {
            var city = token["city"]?.ToString();
            var sellPrice = token["sell_price_min"].ToString();
            if (sellPrice == "0") sellPrice = "-";
            var buyPrice = token["buy_price_max"].ToString();
            if (buyPrice == "0") buyPrice = "-";
            var sellPriceLastUpdate =FormatTimeSpan(DateTime.Now - DateTime.Parse(token["sell_price_min_date"]?.ToString()));
            var buyPriceLastUpdate =FormatTimeSpan(DateTime.Now - DateTime.Parse(token["buy_price_max_date"]?.ToString()));

            var resourceCityModel = new ResourceCityModel
            {
                City = city,
                SellPrice = sellPrice,
                BuyPrice = buyPrice,
                SellPriceLastUpdate = sellPriceLastUpdate,
                BuyPriceLastUpdate = buyPriceLastUpdate
            };

            resourceCityModels.Add(resourceCityModel);
        }
        var maxSellPriceRecord = resourceCityModels.OrderByDescending(r => ParseIntOrReturnMinValue(r.SellPrice)).First();
        maxSellPriceRecord.Note += "[Sell Order Here] ";

        // Find the record with the minimum sell price
        var minSellPriceRecord = resourceCityModels.OrderBy(r => ParseIntOrReturnMaxValue(r.SellPrice)).First();
        minSellPriceRecord.Note += "[Buy Here] ";

        // Find the record with the maximum buy price
        var maxBuyPriceRecord = resourceCityModels.OrderByDescending(r => ParseIntOrReturnMinValue(r.BuyPrice)).First();
        maxBuyPriceRecord.Note += "[Sell Here] ";

        // Find the record with the minimum buy price
        var minBuyPriceRecord = resourceCityModels.OrderBy(r => ParseIntOrReturnMaxValue(r.BuyPrice)).First();
        minBuyPriceRecord.Note += "[Buy Order Here]";
        return resourceCityModels;
    }
    private static async Task<JArray> RequestApiForSingleResource(string itemId)
    {
        var client = new HttpClient();
        var requestUri = $"https://europe.albion-online-data.com/api/v2/stats/prices/{itemId}.json?qualities=1";

        try
        {
            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonArray = JArray.Parse(responseContent);

            return jsonArray;
        }
        catch (HttpRequestException ex)
        {
            throw;
        }
        finally
        {
            client.Dispose();
        }
    }

    static string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 1)
        {
            return "just now";
        }
        else if (timeSpan.TotalMinutes < 1)
        {
            return $"{timeSpan.Seconds} seconds ago";
        }
        else if (timeSpan.TotalHours < 1)
        {
            return $"{timeSpan.Minutes} minutes ago";
        }
        else if (timeSpan.TotalDays < 1)
        {
            return $"{timeSpan.Hours} hours and {timeSpan.Minutes} minutes ago";
        }
        else if(timeSpan.TotalDays < 1000)
        {
            int days = (int)Math.Floor(timeSpan.TotalDays);
            return $"{days} days, {timeSpan.Hours} hours and {timeSpan.Minutes} minutes ago";
        }
        else
        {
            return "Never";
        }
    }
    static int ParseIntOrReturnMinValue(string value)
    {
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        else
        {
            return int.MinValue;
        }
    }

    // Helper method to parse integer or return max value
    static int ParseIntOrReturnMaxValue(string value)
    {
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        else
        {
            return int.MaxValue;
        }
    }
}