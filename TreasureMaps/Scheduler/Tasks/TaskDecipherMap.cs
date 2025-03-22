using ECommons.DalamudServices;
using Dalamud.Game.ClientState.Conditions;
using TreasureMaps.Helpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.Throttlers;
using ECommons.Automation;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskDecipherMap
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Attemping to Decipher Map");
        P.taskManager.Enqueue(Decipher);
    }

    internal unsafe static bool? Decipher()
    {
        if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out var yesNoAddon) && IsAddonReady(yesNoAddon))
        {
            Callback.Fire(yesNoAddon, true, 0);
            Generic.PluginLogInfo("Map Deciphered");
            return true;
        }
        else if (TryGetAddonByName<AtkUnitBase>("SelectIconString", out var iconStringAddon) && IsAddonReady(iconStringAddon))
        {
            if (C.specificMap && C.mapSelected != "Default")
            { 
                if (Generic.FindDecipherIndex(C.mapSelected) == -1)
                {
                    DuoLog.Warning("Map Selected Not In Inventory!");
                    SchedulerMain.DisablePlugin();
                }
                Callback.Fire(iconStringAddon, true, Generic.FindDecipherIndex(C.mapSelected));
            }
            else
            {
                Callback.Fire(iconStringAddon, true, 0);
            }
            return false;
            
        }
        else if (Statuses.PlayerNotBusy() && !Svc.Condition[ConditionFlag.Casting] && EzThrottler.Throttle("OpeningMap"))
        {
            if (C.specificMap && C.mapSelected != "Default" && Inventory.GetMapCount(TreasureMapIds.FindKeysByValue(C.mapSelected).FirstOrDefault()) == 0)
            {
                DuoLog.Warning("Map Selected Not In Inventory!");
                SchedulerMain.DisablePlugin();
            }
            else
            {
                Actions.ExecuteAction(ActionType.GeneralAction, 19);
            }
            return false;
        }
        return false;
    }
}
