using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskOpenDecipheredMap
{
    public static void Enqueue(uint actionId)
    {
        Generic.PluginLogInfo("Opening Map");
        P.taskManager.Enqueue(() => OpenMap(actionId));
    }

    public unsafe static bool? OpenMap(uint actionId)
    {
        var IsMapDeciphered = Inventory.IsMapDeciphered();
        if (IsMapDeciphered && Statuses.PlayerNotBusy() && !Svc.Condition[ConditionFlag.Casting] && 
            AgentMap.Instance()->FlagMapMarker.TerritoryId == AgentMap.Instance()->SelectedTerritoryId && AgentMap.Instance()->IsAddonReady())
        {
            Generic.PluginLogInfo("Map Opened");
            return true;
        }
        else if (IsMapDeciphered && Statuses.PlayerNotBusy() && !Svc.Condition[ConditionFlag.Casting])
        {
            Actions.ExecuteAction(ActionType.KeyItem ,actionId);
            P.mapFinder.OpenMapLocation();
            return false;
        }

        return false;
    }
}
