using AlbionMarketer.Models;
using AlbionMarketer.Static;
using ConsoleTables;

namespace AlbionMarketer.Classes;

public static class ConsoleRenderer
{
    public static async Task<ConsoleKeyInfo> WaitForKeyPress()
    {
        Task<ConsoleKeyInfo> getKeyTask = Task.Run(() => Console.ReadKey(true));
        return await getKeyTask;
    }

    public static void WriteLine(ConsoleColor consoleColor, string s)
    {
        Console.ForegroundColor = consoleColor;
        Console.WriteLine(s);
        Console.ResetColor();
    }

    public static void WriteLine(string s)
    {
        Console.WriteLine(s);
    }

    public static void Option(int highlighted, int nr, string s)
    {
        if (highlighted == nr)
        {
            WriteLine(ConsoleColor.Yellow, s);
        }
        else
        {
            WriteLine(s);
        }
    }

    public static async Task<int> MainMenu()
    {
        int maxMenuIndex = StaticValues.Resources.Count() - 1;
        int highlighted = 0;
        while (true)
        {
            RenderMainMenu(highlighted);
            var key = await WaitForKeyPress();
            if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow)
            {
                return -1;
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    highlighted -= 1;
                    if (highlighted < 0) highlighted = maxMenuIndex;
                    break;
                case ConsoleKey.DownArrow:
                    highlighted += 1;
                    if (highlighted > maxMenuIndex) highlighted = 0;
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:
                    return highlighted;
            }
        }
    }

    public static async Task<int> TierMenu(int resource)
    {
        int maxMenuIndex = StaticValues.Tiers.Count() - 1;
        int highlighted = 0;
        while (true)
        {
            RenderTierMenu(highlighted, resource);
            var key = await WaitForKeyPress();
            if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow)
            {
                return -1;
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    highlighted -= 1;
                    if (highlighted < 0) highlighted = maxMenuIndex;
                    break;
                case ConsoleKey.DownArrow:
                    highlighted += 1;
                    if (highlighted > maxMenuIndex) highlighted = 0;
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:
                    return highlighted;
            }
        }
    }

    public static async Task<string> ItemMenu(Dictionary<string, List<string>> resource, string tier, int resourceNr)
    {
        int maxMenuIndex = resource[tier].Count - 1;
        if (maxMenuIndex == -1)
        {
            RenderEmptyPage(resourceNr, tier);
            Thread.Sleep(2000);
            return "";
        }

        int highlighted = 0;
        while (true)
        {
            RenderItemMenu(highlighted, resource[tier], resourceNr, tier);
            var key = await WaitForKeyPress();
            if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow)
            {
                return "";
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    highlighted -= 1;
                    if (highlighted < 0) highlighted = maxMenuIndex;
                    break;
                case ConsoleKey.DownArrow:
                    highlighted += 1;
                    if (highlighted > maxMenuIndex) highlighted = 0;
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:
                    try
                    {
                        return resource[tier][highlighted];
                    }
                    catch (Exception e)
                    {
                        RenderErrorPage(e);
                        Thread.Sleep(5000);
                        return "";
                    }
            }
        }
    }

    public static void RenderItemMenu(int highlighted, List<string> list, int resourceNr, string tier)
    {
        Console.Clear();
        WriteLine(ConsoleColor.Cyan, StaticValues.BANNER_TOP);
        WriteLine(ConsoleColor.Red,
            StaticValues.BANNER_BOTTOM + $"\nMain Menu->{StaticValues.Resources[resourceNr]}->{tier}\n");
        int iterator = 0;
        foreach (var item in list)
        {
            Option(highlighted, iterator, (iterator + 1) + ". " + item);
            iterator++;
        }
    }

    public static void RenderMainMenu(int highlighted)
    {
        Console.Clear();
        WriteLine(ConsoleColor.Cyan, StaticValues.BANNER_TOP);
        WriteLine(ConsoleColor.Red, StaticValues.BANNER_BOTTOM + "\nMain Menu\n");
        int iterator = 0;
        foreach (var option in StaticValues.Resources)
        {
            Option(highlighted, iterator, (iterator + 1).ToString() + ". " + option);
            iterator++;
        }
    }

    public static void RenderTierMenu(int highlighted, int resourceNr)
    {
        Console.Clear();
        WriteLine(ConsoleColor.Cyan, StaticValues.BANNER_TOP);
        WriteLine(ConsoleColor.Red,
            StaticValues.BANNER_BOTTOM + $"\nMain Menu->{StaticValues.Resources[resourceNr]}\n");
        int iterator = 0;
        foreach (var tier in StaticValues.Tiers)
        {
            Option(highlighted, iterator, (iterator + 1) + ". " + tier);
            iterator++;
        }
    }

    public static void RenderErrorPage(Exception e)
    {
        Console.Clear();
        WriteLine(ConsoleColor.Cyan, StaticValues.BANNER_TOP);
        WriteLine(ConsoleColor.Red, StaticValues.BANNER_BOTTOM + "\n\n");
        WriteLine(ConsoleColor.Red, e.Message);
        Thread.Sleep(5000);
    }

    public static void RenderEmptyPage(int resourceNr, string tier)
    {
        Console.Clear();
        WriteLine(ConsoleColor.Cyan, StaticValues.BANNER_TOP);
        WriteLine(ConsoleColor.Red,
            StaticValues.BANNER_BOTTOM + $"\nMain Menu->{StaticValues.Resources[resourceNr]}->{tier}\n");
        WriteLine(ConsoleColor.Red, "This Tier is empty !");
    }

    public static void RenderItemDataPage(string item, string tierName, int resource,
        List<ResourceCityModel> itemPrices)
    {
        Console.Clear();
        WriteLine(ConsoleColor.Cyan, StaticValues.BANNER_TOP);
        WriteLine(ConsoleColor.Red,
            StaticValues.BANNER_BOTTOM + $"\nMain Menu->{StaticValues.Resources[resource]}->{tierName}->{item}\n");
        WriteLine(ConsoleColor.Cyan, "Loaded");
        var table = new ConsoleTable("City", "Sell Price", "Buy Price", "Sell Price Updated At", "Buy Price Updated At",
            "Note");
        foreach (var city in itemPrices)
        {
            table.AddRow(city.City, city.SellPrice, city.BuyPrice, city.SellPriceLastUpdate, city.BuyPriceLastUpdate,
                city.Note);
        }

        // Print the table to console
        table.Write(Format.Minimal);
    }

    public static void RenderItemAwait(string item, string tierName, int resource, int iteration)
    {
        Console.Clear();
        WriteLine(ConsoleColor.Cyan, StaticValues.BANNER_TOP);
        WriteLine(ConsoleColor.Red,
            StaticValues.BANNER_BOTTOM + $"\nMain Menu->{StaticValues.Resources[resource]}->{tierName}->{item}\n");
        WriteLine(ConsoleColor.Cyan, StaticValues.arrow[iteration]);
    }

    public static async Task<bool> ItemData(string item, string tierName, int resource)
    {
        while (true)
        {
            try
            {
                var itemPrices = ApiDataModule.GetItemPrices(item);
                var iterator = 0;
                while (!itemPrices.IsCompleted)
                {
                    RenderItemAwait(item, tierName, resource, iterator);
                    iterator++;
                    if (iterator > 4) iterator = 0;
                    await Task.Delay(200);
                }
                RenderItemDataPage(item, tierName, resource, itemPrices.Result);
            }
            catch (Exception e)
            {
                RenderErrorPage(e);
                return true;
            }
            var key = await WaitForKeyPress();
            switch (key.Key)
            {
                case ConsoleKey.Escape or ConsoleKey.LeftArrow:
                    return true;
                case ConsoleKey.RightArrow or ConsoleKey.Enter:
                    return false;
            }
        }
    }
}