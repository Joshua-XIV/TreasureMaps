using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.EzHookManager;
using ECommons.GameHelpers;

namespace TreasureMaps.Helpers;

public static class Statuses
{
    // Delegate for retrieving the nameplate color of a game object.
    private delegate byte GetNameplateColorDelegate(nint ptr);
    // Native function to get the nameplate color of a game object.
    private static GetNameplateColorDelegate GetNameplateColorNative;
    // Signature for the native function that retrieves the nameplate color.
    private static string GetNameplateColorSig = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 35 ?? ?? ?? ?? 48 8B F9";

    /// <summary>
    /// Checks if the player is not busy and available to perform actions.
    /// </summary>
    /// <returns>
    /// True if the player is available, not casting, not jumping, targetable, and not animation-locked; otherwise, false.
    /// </returns>
    public static bool PlayerNotBusy()
    {
        return Player.Available
               && Player.Object.CastActionId == 0
               && !IsOccupied()
               && !Svc.Condition[ConditionFlag.Jumping]
               && Player.Object.IsTargetable
               && !Player.IsAnimationLocked;
    }

    /// <summary>
    /// Checks if the player is currently in combat.
    /// </summary>
    /// <returns>True if the player is in combat, otherwise false.</returns>
    public static bool InCombat()
    {
        return Svc.Condition[ConditionFlag.InCombat];
    }

    /// <summary>
    /// Checks if the specified game object is an engaged mob (e.g., red or orange nameplate).
    /// </summary>
    /// <param name="gameObject">The game object to check.</param>
    /// <returns>True if the object is engaged with the player's party (red or orange nameplate), otherwise false.</returns>
    public static bool IsEngagedMob(this IGameObject gameObject)
    {
        GetNameplateColorNative ??= EzDelegate.Get<GetNameplateColorDelegate>(GetNameplateColorSig);
        var plateType = GetNameplateColorNative(gameObject.Address);
        //9: red, engaged with your party
        //11: orange, aggroed to your party but not attacked yet
        return plateType == 9 || plateType == 11;
    }
}