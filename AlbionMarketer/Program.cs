using AlbionMarketer.Classes;
using AlbionMarketer.Static;

while (true)
{
    mainMenu:
    int resource = await ConsoleRenderer.MainMenu();
    if (resource == -1) return 1;
        
    tierMenu:
    int tier = await ConsoleRenderer.TierMenu(resource);
    if (tier == -1) goto mainMenu;
    
    itemMenu:
    Dictionary<string, List<string>> resourceDictionary = StaticValues.GetResourceDictionary(resource);
    string tierName = StaticValues.Tiers[tier];
    string item = await ConsoleRenderer.ItemMenu(resourceDictionary,tierName,resource);
    if (item == "")goto tierMenu;

    if(await ConsoleRenderer.ItemData(item, tierName, resource))goto itemMenu;
}