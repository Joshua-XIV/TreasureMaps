using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskMount
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Mounting");
        P.taskManager.Enqueue(Mount);
    }

    internal unsafe static bool? Mount()
    {
        if (Svc.Condition[ConditionFlag.Mounted] && Statuses.PlayerNotBusy()) return true;

        if (!Svc.Condition[ConditionFlag.Casting] && !Svc.Condition[ConditionFlag.Unknown57] && Statuses.PlayerNotBusy())
        {
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, 24);
        }

        return false;
    }
}
