
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace TreasureMaps.Helpers;

public unsafe static class Actions
{
    /// <summary>
    /// Executes the specified action using the ActionManager.
    /// </summary>
    /// <param name="actionType">The type of the action (GeneralAction, KeyAction, EventAction).</param>
    /// <param name="actionId">The ID of the action to execute.</param>
    public static unsafe void ExecuteAction(ActionType actionType, uint actionId) => ActionManager.Instance()->UseAction(actionType, actionId);

    /// <summary>
    /// Checks if the specified action is off cooldown through the ActionManager.
    /// </summary>
    /// <param name="actionType">The type of the action to check (GeneralAction, KeyAction, EventAction).</param>
    /// <param name="actionId">The ID of the action to check.</param>
    /// <returns>True if the action is off cooldown, otherwise false.</returns>
    public static bool IsActionOffCooldown(ActionType actionType, uint actionId)
    {
        return ActionManager.Instance()->IsActionOffCooldown(actionType, actionId);
    }

    /// <summary>
    /// Checks if equipped gear is under certain threshold.
    /// </summary>
    /// <param name="below">The threshold given to repair gear under condition, default as 0</param>
    /// <returns>True if gear is met at threshold, otherwise false</returns>
    public static unsafe bool NeedsRepair(float below = 0)
    {
        var im = InventoryManager.Instance();
        if (im == null)
        {
            Svc.Log.Error("InventoryManager was null");
            return false;
        }

        var equipped = im->GetInventoryContainer(InventoryType.EquippedItems);
        if (equipped == null)
        {
            Svc.Log.Error("InventoryContainer was null");
            return false;
        }

        if (!equipped->IsLoaded)
        {
            Svc.Log.Error($"InventoryContainer is not loaded");
            return false;
        }

        for (var i = 0; i < equipped->Size; i++)
        {
            var item = equipped->GetInventorySlot(i);
            if (item == null)
                continue;

            var itemCondition = Convert.ToInt32(Convert.ToDouble(item->Condition) / 30000.0 * 100.0);

            if (itemCondition <= below)
                return true;
        }

        return false;
    }
}
