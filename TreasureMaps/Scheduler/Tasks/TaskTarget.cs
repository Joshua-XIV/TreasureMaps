using Dalamud.Game.ClientState.Objects.Types;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskTarget
{
    public static void Enqueue(string targetName)
    {
        IGameObject? gameobject = null;
        P.taskManager.Enqueue(() => Targeting.TryGetObjectByName(targetName, out gameobject));
        Generic.PluginLogInfo($"Targeting {gameobject!.Name}");
        P.taskManager.Enqueue(() => Targeting.TargetByName(gameobject, targetName) || !Targeting.DoesObjectExist(gameobject.Name.ToString()));
    }

    public static void Enqueue(IGameObject gameObject)
    {
        Generic.PluginLogInfo($"Targeting {gameObject!.Name}");
        P.taskManager.Enqueue(() => Targeting.TargetByObject(gameObject));
    }
}
