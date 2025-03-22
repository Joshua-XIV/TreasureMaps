using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskJumpFly
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Flight");
        P.taskManager.Enqueue(JumpFly);
    }

    internal unsafe static bool? JumpFly()
    {
        if (Svc.Condition[ConditionFlag.InFlight] && Svc.Condition[ConditionFlag.Mounted] && Statuses.PlayerNotBusy()) return true;

        if (!Svc.Condition[ConditionFlag.InFlight] && Svc.Condition[ConditionFlag.Mounted] && Statuses.PlayerNotBusy())
        {
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, 2);
            return false;
        }

        if (!Svc.Condition[ConditionFlag.Casting] && !Svc.Condition[ConditionFlag.Unknown57] && Statuses.PlayerNotBusy())
        {
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, 24);
            return false;
        }

        return false;
    }
}
