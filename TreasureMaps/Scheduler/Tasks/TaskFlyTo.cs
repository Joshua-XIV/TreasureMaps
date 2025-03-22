using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using System.Numerics;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskFlyTo
{
    public static void Enqueue(string destination)
    {
        Generic.PluginLogInfo($"Moving to {destination}");
        P.taskManager.Enqueue(() => FlyTo(), 1000*60*3, false);
    }

    private static Vector3? lastPosition = null;
    private static DateTime lastParsedTime = DateTime.MinValue;

    internal unsafe static bool? FlyTo()
    {
        if (!P.navmesh.IsRunning() && !P.navmesh.PathfindInProgress())
        {
            return true;
        }

        if (P.navmesh.PathfindInProgress() || P.navmesh.IsRunning() || Movement.IsMoving())
        {
            var currentPosition = Svc.ClientState.LocalPlayer.Position;
            if ((DateTime.Now - lastParsedTime).TotalSeconds >= 10)
            {
                if (lastPosition == null)
                {
                    lastPosition = currentPosition;
                }
                else
                {
                    if (Vector3.Distance(lastPosition.Value, currentPosition) < 5f)
                    {
                        PluginLog.Information("Positional has not changed for 5 seconds, exiting function");
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
                lastPosition = currentPosition;
                lastParsedTime = DateTime.Now;
            }
        }
        P.navmesh.SetAlignCamera(true);
        return false;
    }
}
