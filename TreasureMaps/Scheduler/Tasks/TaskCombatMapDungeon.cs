using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskCombatMapDungeon
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Doing Combat");
        P.taskManager.Enqueue(Combat, 1000 * 60 * 3, false);
    }

    public static void Enqueue(string message)
    {
        Generic.PluginLogInfo("Doing Combat");
        P.taskManager.Enqueue(() => Combat(message), 1000 * 60 * 5, false);
    }

    internal unsafe static bool? Combat()
    {
        IGameObject gameObject = null;
        if (!Statuses.InCombat() && Statuses.PlayerNotBusy() && !Targeting.TryGetAnyClosestEnemy(out gameObject))
        {
            return true;
        }
        else if (Targeting.TryGetAnyClosestEnemy(out gameObject))
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

    internal unsafe static bool? Combat(string message)
    {
        IGameObject gameObject = null;
        if (!Statuses.InCombat() && Statuses.PlayerNotBusy() && !Targeting.TryGetAnyClosestEnemy(out gameObject) && Generic.GetToDoSubText() != message)
        {
            return true;
        }
        else if (Targeting.TryGetAnyClosestEnemy(out gameObject))
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
