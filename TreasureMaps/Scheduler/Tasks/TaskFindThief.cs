using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskFindThief
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Finding Thief");
        P.taskManager.Enqueue(() => GoUnderWater());
        P.taskManager.Enqueue(() => FindThief(), 1000*60);
    }

    private static float lastPosition = float.MinValue;
    private static DateTime lastParsedTime = DateTime.MinValue;


    public unsafe static bool GoUnderWater()
    {
        if (Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.Diving])
        {
            return true;
        }
        else
        {
            if (EzThrottler.Throttle("Dive", 1000*2))
                Chat.Instance.SendMessage("/vnav flydir 0 -10 0");
        }
        return false;
    }
    public unsafe static bool FindThief()
    {
        if (!P.navmesh.IsRunning() && !P.navmesh.PathfindInProgress())
        {
            if (EzThrottler.Throttle("Dive", 1000 * 3))
                Chat.Instance.SendMessage("/vnav flydir 0 -20 0");
        }

        if (P.navmesh.PathfindInProgress() || P.navmesh.IsRunning() || Movement.IsMoving())
        {
            var currentPosition = Svc.ClientState.LocalPlayer.Position;
            if ((DateTime.Now - lastParsedTime).TotalSeconds >= 2)
            {
                if (lastPosition == float.MinValue)
                {
                    lastPosition = currentPosition.Y;
                }
                else
                {
                    if (Math.Abs(lastPosition - currentPosition.Y) < 3f)
                    {
                        PluginLog.Information("Positional has not changed");
                        return true;
                    }
                    else
                    {
                        if (C.DEBUG)
                        {
                            PluginLog.Information("Position has changed");
                        }
                    }
                }
                lastPosition = currentPosition.Y;
                lastParsedTime = DateTime.Now;
            }
        }
        P.navmesh.SetAlignCamera(true);
        return false;
    }
}
