using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Numerics;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskMoveTo
{
    public static void Enqueue(Vector3 targetPosition, string destination, float minDistance = 3f)
    {
        Generic.PluginLogInfo($"Moving to {destination}");
        P.taskManager.Enqueue(() => MoveTo(targetPosition, minDistance));
    }

    internal unsafe static bool? MoveTo(Vector3 targetPosition, float minDistance = 3f)
    {
        if (Distance.GetDistanceToPlayer(targetPosition) <= minDistance)
        {
            P.navmesh.Stop();
            return true;
        }

        if (Movement.IsMoving() && Distance.GetDistanceToPlayer(targetPosition) >= 6)
        {
            if (ActionManager.Instance()->GetActionStatus(ActionType.GeneralAction, 4) == 0 && ActionManager.Instance()->QueuedActionId != 4 && !Player.Character->IsCasting)
                ActionManager.Instance()->UseAction(ActionType.GeneralAction, 4);
        }

        if (P.navmesh.PathfindInProgress() || P.navmesh.IsRunning() || Movement.IsMoving()) return false;

        P.navmesh.PathfindAndMoveTo(targetPosition, false);
        P.navmesh.SetAlignCamera(true);
        return false;
    }
}
