using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskStartTreasureHunt
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Starting Hunt");
        P.taskManager.Enqueue(Start, 1000*2);
    }

    internal unsafe static bool? Start()
    {
        if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out var yesNoAddon) && IsAddonReady(yesNoAddon))
        {
            Callback.Fire(yesNoAddon, true, 0);
            return true;
        }

        if (Statuses.InCombat())
        {
            return true;
        }

        return false;
    }
}
