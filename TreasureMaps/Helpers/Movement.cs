using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace TreasureMaps.Helpers;

public unsafe class Movement
{
    /// <summary>
    /// Checks if the player is currently moving.
    /// </summary>
    /// <returns>True if the player is moving, otherwise false.</returns>
    public static unsafe bool IsMoving() => AgentMap.Instance()->IsPlayerMoving;
}
