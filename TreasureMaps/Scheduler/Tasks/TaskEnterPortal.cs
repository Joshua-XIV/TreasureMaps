using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskSelectYes
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Selecting Yes...");
        P.taskManager.Enqueue(Enter, 5000);
    }

    internal unsafe static bool? Enter()
    {
        if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out var yesNoAddon) && IsAddonReady(yesNoAddon))
        {
            Callback.Fire(yesNoAddon, true, 0);
            return true;
        }

        return false;
    }
}
