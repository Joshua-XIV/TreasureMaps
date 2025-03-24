using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using TreasureMaps.Helpers;
namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskTeleport
{
    public static void Enqueue(uint aetheryteId, uint targetZoneId)
    {
        Generic.PluginLogInfo($"Teleporting to {Zones.GetZoneName(targetZoneId)}");
        P.taskManager.Enqueue(() => TeleportToAetheryte(aetheryteId, targetZoneId));
    }

    internal static unsafe bool? TeleportToAetheryte(uint aetheryteId, uint targetZoneId)
    {
        if (Zones.IsInZone(targetZoneId) && Statuses.PlayerNotBusy()) return true;

        if (!Svc.Condition[ConditionFlag.Casting] && Statuses.PlayerNotBusy() && EzThrottler.Throttle("Teleporting", 300) &&
            !Svc.Condition[ConditionFlag.BetweenAreas] && !Zones.IsInZone(targetZoneId))
        {
            Telepo.Instance()->Teleport(aetheryteId, 0);
            return false;
        }

        return false;
    }
}
