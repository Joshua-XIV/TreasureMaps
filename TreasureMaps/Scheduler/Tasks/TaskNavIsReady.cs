using ECommons.Throttlers;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskNavIsReady
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Checking if Nav is ready...");
        P.taskManager.Enqueue(IsReady, 1000*60*3, false);
    }

    internal unsafe static bool? IsReady()
    {
        if (P.navmesh.IsReady() && EzThrottler.Throttle("Nav")) return true;
        else return false;
    }
}
