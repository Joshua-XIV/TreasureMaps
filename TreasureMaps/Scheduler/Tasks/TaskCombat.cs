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

internal static class TaskCombat
{
    public static void Enqueue()
    {
        Generic.PluginLogInfo("Doing Combat");
        P.taskManager.Enqueue(Combat, 1000 * 60 * 3, false);
    }

    internal unsafe static bool? Combat()
    {
        if (!Statuses.InCombat() && Statuses.PlayerNotBusy())
        {
            return true;
        }
        if (Statuses.InCombat())
        {
            IGameObject gameObject = null;
            if (Targeting.TryGetClosestEnemy(out gameObject))
            {
                Svc.Targets.SetTarget(gameObject);
            }
        }
        return false;
    }
}
