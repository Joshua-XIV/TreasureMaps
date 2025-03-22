using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace TreasureMaps.Helpers;

public static class Inventory
{
    /// <summary>
    /// Searches through the "Key Items" inventory to find any valid treasure map that is already deciphered.
    /// </summary>
    /// <returns>True if a deciphered treasure map is found, otherwise false.</returns>
    public static bool IsMapDeciphered()
    {
        var inventory = Svc.GameInventory.GetInventoryItems(Dalamud.Game.Inventory.GameInventoryType.KeyItems);
        foreach (var item in inventory)
        {
            if (DecipheredTreasureMapIds.ContainsKey(item.ItemId))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Searches through the "Key Items" inventory to find any valid treasure map that is already deciphered.
    /// </summary>
    /// <returns>If a deciphered map is found, returns the item's ID; otherwise, returns 0.</returns>
    public static uint GetMapDecipheredId()
    {
        var inventory = Svc.GameInventory.GetInventoryItems(Dalamud.Game.Inventory.GameInventoryType.KeyItems);
        foreach (var item in inventory)
        {
            if (DecipheredTreasureMapIds.ContainsKey(item.ItemId))
            {
                return item.ItemId;
            }
        }
        return 0;
    }

    /// <summary>
    /// Searches through the complete main inventory (Inventory1-4) to find any valid treasure map.
    /// </summary>
    /// <returns>True if a treasure map is found, otherwise false.</returns>
    public static bool HasMap()
    {
        var inventories = new[]
        {
        Dalamud.Game.Inventory.GameInventoryType.Inventory1,
        Dalamud.Game.Inventory.GameInventoryType.Inventory2,
        Dalamud.Game.Inventory.GameInventoryType.Inventory3,
        Dalamud.Game.Inventory.GameInventoryType.Inventory4
        };

        foreach (var inventoryType in inventories)
        {
            var inventory = Svc.GameInventory.GetInventoryItems(inventoryType);
            foreach (var item in inventory)
            {
                if (TreasureMapIds.ContainsKey(item.ItemId))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieves the count of a specific item in the inventory. Main purpose is for a specific map
    /// </summary>
    /// <param name="itemID">The ID of the item to count.</param>
    /// <returns>The count of the specified item in the inventory.</returns>
    public unsafe static int GetMapCount(uint itemID) => InventoryManager.Instance()->GetInventoryItemCount(itemID);

    /// <summary>
    /// Calculates the total count of all valid treasure maps in the inventory.
    /// </summary>
    /// <returns>The total count of all treasure maps in the inventory.</returns>
    public static int GetTotalMapCount()
    {
        int sum = 0;
        foreach (var x in TreasureMapIds)
        {
            sum += GetMapCount(x.Key);
        }
        return sum;
    }

    /// <summary>
    /// Searches through the complete main inventory (Inventory1-4) to find all valid treasure maps.
    /// </summary>
    /// <returns>A list of strings representing each treasure map in the current character's inventory.</returns>
    public static List<string> GetAllMaps()
    {
        List<string> list = new List<string>();
        list.Add("Default");
        var inventories = new[]
        {
        Dalamud.Game.Inventory.GameInventoryType.Inventory1,
        Dalamud.Game.Inventory.GameInventoryType.Inventory2,
        Dalamud.Game.Inventory.GameInventoryType.Inventory3,
        Dalamud.Game.Inventory.GameInventoryType.Inventory4
        };

        foreach (var inventoryType in inventories)
        {
            var inventory = Svc.GameInventory.GetInventoryItems(inventoryType);
            foreach (var item in inventory)
            {
                if (TreasureMapIds.ContainsKey(item.ItemId))
                {
                    list.Add(TreasureMapIds.GetOrDefault(item.ItemId));
                }
            }
        }
        return list;
    }
}