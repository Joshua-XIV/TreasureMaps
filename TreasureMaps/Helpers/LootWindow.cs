using FFXIVClientStructs.FFXIV.Component.GUI;
namespace TreasureMaps.Helpers;

public static class LootWindow
{
    /// <summary>
    /// Checks if the loot notification window ("_NotificationLoot") is currently open.
    /// </summary>
    /// <returns>True if the loot notification window is open, otherwise false.</returns>
    public unsafe static bool hasLootNotifitcation()
    {
        TryGetAddonByName<AtkUnitBase>("_NotificationLoot", out var lootWindow);
        if (lootWindow != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
