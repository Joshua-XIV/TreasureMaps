using Dalamud.Game.ClientState.Objects.Types;
using TreasureMaps.Helpers;

namespace TreasureMaps.Scheduler.Tasks;

internal static class TaskInteract
{
    public static void Enqueue(string objectName)
    {
        IGameObject? gameObject = null;
        P.taskManager.Enqueue(() => Targeting.TryGetObjectByName(objectName, out gameObject));
        Generic.PluginLogInfo($"Interacting with {gameObject!.Name}");
        P.taskManager.Enqueue(() => Targeting.InteractWithObject(gameObject), 1000*2);
    }

    public static void Enqueue(IGameObject gameObject)
    {
        Generic.PluginLogInfo($"Interacting with {gameObject!.Name}");
        P.taskManager.Enqueue(() => Targeting.InteractWithObject(gameObject), 1000 * 2);
    }
}
