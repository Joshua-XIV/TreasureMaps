using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskDisMount
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("DisMounting");
        P.taskManager.Enqueue(DisMount);
    }

    internal unsafe static bool? DisMount()
    {
        if (!Svc.Condition[ConditionFlag.Mounted] && Statuses.PlayerNotBusy()) return true;

        if (Svc.Condition[ConditionFlag.Mounted] && Statuses.PlayerNotBusy())
        {
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, 23);
            return false;
        }

        return false;
    }
}
