﻿using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using ECommons.Automation;
using ECommons.DalamudServices;
using Lumina.Data.Files.Excel;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskGoNextFloor
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Moving Forward");
        P.taskManager.Enqueue(MoveForwardUntilCondition);
    }

    internal unsafe static bool? MoveForwardUntilCondition()
    {
        if (Svc.Condition[ConditionFlag.Jumping61] || Svc.Condition[ConditionFlag.Jumping])
        {
            WindowsKeypress.SendKeyRelease((VirtualKey)C.walkForwardKey, null);
            return true;
        }
        else if (Movement.IsMoving())
        {
            return false;
        }
        else
        {
            WindowsKeypress.SendKeyHold((VirtualKey)C.walkForwardKey, null);
        }
        return false;
    }
}
