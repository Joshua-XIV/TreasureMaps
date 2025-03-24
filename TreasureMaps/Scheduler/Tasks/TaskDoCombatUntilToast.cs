using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskDoCombatUntilToast
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Doing Treasure Hunt Location");
        // Ruby Sea - 613
        // BMR doesn't play well in this zone
        if (!Zones.IsInZone(613))
        {
            P.taskManager.Enqueue(() => PluginManager.EnableBossMod());
        }
        P.taskManager.Enqueue(() => Toast(), 1000 * 60 * 5, false);
        P.taskManager.Enqueue(() => PluginManager.DisableBossMod());
    }

    internal static string PrintTextToast()
    {
        var lastToast = P.toast.GetLastToast();
        if (lastToast == null) return null;
        return lastToast;
    }

    internal unsafe static bool? Toast()
    {
        if (CompleteTreasureLocationToast.Contains(PrintTextToast()) || !Svc.Condition[ConditionFlag.BoundByDuty]) return true;

        IGameObject gameObject = null;
        if (Targeting.TryGetClosestTerritorialEnemy(out gameObject))
        {
            if (gameObject != null && !gameObject.IsDead && gameObject.IsTarget())
            {
                if (gameObject != null && Distance.DistanceToHitboxEdge(gameObject.HitboxRadius, gameObject) > Distance.GetRange() && !C.bossModRebornPlugin)
                {
                    if (!P.navmesh.PathfindInProgress() && !P.navmesh.IsRunning() && !Movement.IsMoving() && !gameObject.IsDead && Distance.DistanceToHitboxEdge(gameObject.HitboxRadius, gameObject) > Distance.GetRange())
                    {
                        P.navmesh.PathfindAndMoveTo(gameObject.Position, false);
                    }
                    return false;
                }
                else
                {
                    P.navmesh.Stop();
                    return false;
                }
            }
            else
            {
                Svc.Targets.SetTarget(gameObject);
            }
        }
        return false;
    }
}
