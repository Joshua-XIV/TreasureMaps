using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskDigMap
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Digging");
        P.taskManager.Enqueue(Dig);
    }

    internal unsafe static bool? Dig()
    {
        if (Svc.Condition[ConditionFlag.BoundByDuty] && !Actions.IsActionOffCooldown(ActionType.GeneralAction, 20) && Statuses.PlayerNotBusy() && Targeting.FindChest(out var gameObject)) return true;

        if (Actions.IsActionOffCooldown(ActionType.GeneralAction, 20) && !Svc.Condition[ConditionFlag.Mounted] && EzThrottler.Throttle("Dig"))
        {
            Actions.ExecuteAction(ActionType.GeneralAction, 20);
            return false;
        }

        return false;
    }
}
