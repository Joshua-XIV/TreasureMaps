
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using System.Numerics;

namespace TreasureMaps.Helpers;

public static class Distance
{
    /// <summary>
    /// Calculates the 2D distance between two positions using their X and Y coordinates.
    /// </summary>
    /// <param name="firstPosition">The first position as a Vector3.</param>
    /// <param name="secondPosition">The second position as a Vector3.</param>
    /// <returns>The 2D distance between the two positions.</returns>
    public static float DistanceBetweenPositions(this Vector3 firstPosition, Vector3 secondPosition)
    {
        return new Vector2(firstPosition.X - secondPosition.X, firstPosition.Y - secondPosition.Y).Length();
    }

    /// <summary>
    /// Calculates the 3D distance between a given position and the player's current position.
    /// </summary>
    /// <param name="v3">The position to calculate the distance from.</param>
    public static unsafe float GetDistanceToPlayer(Vector3 v3) => Vector3.Distance(v3, Player.GameObject->Position);

    /// <param name="gameObject">The game object to calculate the distance from.</param>
    public static unsafe float GetDistanceToPlayer(IGameObject gameObject) => GetDistanceToPlayer(gameObject.Position);

    /// <param name="pointFloor">The nullable position to calculate the distance from.</param>
    public static unsafe float GetDistanceToPlayer(Vector3? v3)
    {
        if (v3.HasValue)
        {
            return Vector3.Distance(v3.Value, Player.GameObject->Position);
        }
        else
        {
            return 0;
        }
    }
    /// <returns>The 3D distance between the given position and the player's position.</returns>

    /// <summary>
    /// Calculates the distance from the player to the edge of a game object's hitbox.
    /// </summary>
    /// <param name="hitboxRadius">The radius of the game object's hitbox.</param>
    /// <param name="gameObject">The game object to calculate the distance to.</param>
    /// <returns>The distance from the player to the edge of the game object's hitbox.</returns>
    public static unsafe float DistanceToHitboxEdge(float hitboxRadius, IGameObject gameObject) => GetDistanceToPlayer(gameObject) - hitboxRadius;

    /// <summary>
    /// Determines the appropriate range for the player's current class/job. This is used to determine the maximum distance the player should be from targets.
    /// </summary>
    /// <returns>By default, returns 2.8, which is considered a safe melee range. For classes that do not require close proximity for AoE, returns 15.</returns>
    public static float GetRange()
    {
        var x = Svc.ClientState.LocalPlayer!.ClassJob.RowId;
        var range = 2.8f;
        switch (x)
        {
            // All classes without close range AOE
            case 7 or 25 or 33 or 35 or 42 or 26 or 27:
                range = 15;
                break;

            default:
                range = 2.8f;
                break;
        }
        return range;
    }
}
