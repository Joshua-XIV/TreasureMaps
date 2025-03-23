using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ECommons.GameFunctions;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Data.Parsing.Layer;

namespace TreasureMaps.Helpers;

public static class Targeting
{
    /// <summary>
    /// Attempts to find the closest engaged enemy (red or orange nameplate) that is targetable and not dead.
    /// </summary>
    /// <param name="gameObject">The closest engaged enemy, if found; otherwise, null.</param>
    /// <returns>True if an engaged enemy is found, otherwise false.</returns>
    public static bool TryGetClosestEnemy(out IGameObject? gameObject)
        => (gameObject = Svc.Objects.OrderBy(Distance.GetDistanceToPlayer).FirstOrDefault(x => x.IsTargetable && Statuses.IsEngagedMob(x) && !x.IsDead)) != null;


    /// <summary>
    /// Attempts to find the closest hostile enemy that is targetable and not dead.
    /// </summary>
    /// <param name="gameObject">The closest hostile enemy, if found; otherwise, null.</param>
    /// <returns>True if a hostile enemy is found, otherwise false.</returns>
    public static bool TryGetAnyClosestEnemy(out IGameObject? gameObject)
        => (gameObject = Svc.Objects.OrderBy(Distance.GetDistanceToPlayer).FirstOrDefault(x => x.IsTargetable && x.IsHostile() && !x.IsDead)) != null;

    /// <summary>
    /// Attempts to find the closest territorial enemy (e.g., treasure hunt mobs) that is targetable and not dead.
    /// </summary>
    /// <param name="gameObject">The closest territorial enemy, if found; otherwise, null.</param>
    /// <returns>True if a territorial enemy is found, otherwise false.</returns>
    public unsafe static bool TryGetClosestTerritorialEnemy(out IGameObject? gameObject)
    {
        var target = Svc.Objects.FirstOrDefault(x =>
        {
            if (x == null) return false;
            if (x.IsDead) return false;
            var address = (GameObject*)(void*)x.Address;
            if (address->ObjectKind != ObjectKind.BattleNpc) return false;
            if (address->SubKind != 5) return false;
            if (address->EventId.ContentId != EventHandlerType.TreasureHuntDirector) return false;
            if (address->NamePlateIconId != 60094 && address->NamePlateIconId != 60096) return false;
            return true;
        });

        if (target == null)
        { 
            gameObject = null;
            return false;
        }
        gameObject = target;
        return true;
    }

    /// <summary>
    /// Attempts to find the closest game object by name.
    /// </summary>
    /// <param name="name">The name of the object to search for.</param>
    /// <param name="gameObject">The closest matching object, if found; otherwise, null.</param>
    /// <returns>True if a matching object is found, otherwise false.</returns>
    public static bool TryGetObjectByName(string name, out IGameObject? gameObject)
        => (gameObject = Svc.Objects.OrderBy(Distance.GetDistanceToPlayer).FirstOrDefault(x => x.Name.ToString() == name)) != null;


    /// <summary>
    /// Checks if a game object with the specified name exists and is targetable.
    /// </summary>
    /// <param name="name">The name of the object to check.</param>
    /// <returns>True if the object exists and is targetable, otherwise false.</returns>
    public static bool DoesObjectExist(string name) => TryGetObjectByName(name, out IGameObject? gameObject) && gameObject.IsTargetable;

    /// <summary>
    /// Sets the target to the specified game object by name.
    /// </summary>
    /// <param name="gameObject">The game object to target.</param>
    /// <param name="name">The name of the object to target.</param>
    /// <returns>True if the target is successfully set, otherwise false.</returns>
    public static bool TargetByName(IGameObject? gameObject, string name)
    {
        if (Svc.Targets.Target != null && Svc.Targets.Target.Name.ToString() == name) return true;

        if (gameObject != null)
        {
            if (EzThrottler.Throttle("Targeting"))
            {
                Svc.Targets.SetTarget(gameObject);
                PluginLog.Information($"Targeting {gameObject.Name}");
            }
        }
        return false;
    }

    /// <summary>
    /// Sets the target to the specified game object.
    /// </summary>
    /// <param name="gameObject">The game object to target.</param>
    /// <returns>True if the target is successfully set, otherwise false.</returns>
    public static bool TargetByObject(IGameObject? gameObject)
    {
        if (Svc.Targets.Target != null && Svc.Targets.Target.GameObjectId == gameObject.GameObjectId) return true;

        if (gameObject != null)
        {
            if (EzThrottler.Throttle("Targeting"))
            {
                Svc.Targets.SetTarget(gameObject);
                PluginLog.Information($"Targeting {gameObject.Name}");
            }
        }
        return false;
    }

    /// <summary>
    /// Attempts to interact with the specified game object.
    /// </summary>
    /// <param name="gameObject">The game object to interact with.</param>
    public static unsafe void InteractWithObject(IGameObject? gameObject)
    {
        try
        {
            if (gameObject == null || !gameObject.IsTargetable)
                return;
            var gameObjectPointer = (GameObject*)gameObject.Address;
            // false to ignore line of sight
            TargetSystem.Instance()->InteractWithObject(gameObjectPointer, false);
        }
        catch (Exception ex)
        {
            Svc.Log.Info($"InteractWithObject: Exception: {ex}");

        }
    }

    /// <summary>
    /// Attempts to find a treasure chest in the current area.
    /// </summary>
    /// <param name="gameObject">The treasure chest, if found; otherwise, null.</param>
    /// <returns>True if a treasure chest is found, otherwise false.</returns>
    public unsafe static bool FindChest(out IGameObject gameObject)
    {
        var treasure = Svc.Objects.FirstOrDefault(o =>
        {
            if (o == null) return false;
            var postion = new Vector3(o.Position.X, o.Position.Y, o.Position.Z);
            var address = (GameObject*)(void*)o.Address;
            if (address->ObjectKind != ObjectKind.Treasure) return false;
            if (address->EventId.ContentId != EventHandlerType.TreasureHuntDirector) return false;
            return true;
        });

        if (treasure == null)
        {
            gameObject = null;
            return false;
        }
        gameObject = treasure;
        return true;
    }

    /// <summary>
    /// Attempts to find a treasure chest that initiates the floor of the terasure dungeon.
    /// </summary>
    /// <param name="gameObject">The treasure chest, if found; otherwise, null.</param>
    /// <returns>True if a treasure chest is found, otherwise false.</returns>
    public unsafe static bool FindStartFloorChest(out IGameObject gameObject)
    {
        var treasure = Svc.Objects.FirstOrDefault(o =>
        {
            if (o == null  || !o.IsTargetable) return false;
            var postion = new Vector3(o.Position.X, o.Position.Y, o.Position.Z);
            var address = (GameObject*)(void*)o.Address;
            if (address->ObjectKind != ObjectKind.EventObj) return false;
            if (address->EventId.ContentId != EventHandlerType.InstanceContentDirector) return false;
            return true;
        });

        if (treasure == null)
        {
            gameObject = null;
            return false;
        }
        gameObject = treasure;
        return true;
    }

    /// <summary>
    /// Attempts to find all lootable objects in the current area.
    /// </summary>
    /// <param name="gameObjects">An array of lootable objects, if found; otherwise, an empty array.</param>
    /// <returns>True if lootable objects are found, otherwise false.</returns>
    public unsafe static bool FindAllLoot(out IGameObject[] gameObjects)
    {
        var treasure = Svc.Objects.Where(o =>
        {
            if (o == null) return false;
            var postion = new Vector3(o.Position.X, o.Position.Y, o.Position.Z);
            var address = (GameObject*)(void*)o.Address;
            if (address->ObjectKind != ObjectKind.Treasure) return false;
            return true;
        }).ToArray();

        if (treasure.Length == 0)
        {
            gameObjects = Array.Empty<IGameObject>();
            return false;
        }
        gameObjects = treasure;
        return true;
    }

    /// <summary>
    /// Attempts to find a portal in the current area.
    /// </summary>
    /// <param name="gameObject">The portal, if found; otherwise, null.</param>
    /// <returns>True if a portal is found, otherwise false.</returns>
    public unsafe static bool FindPortal(out IGameObject gameObject)
    {
        var portal = Svc.Objects.FirstOrDefault(o =>
        {
            if (o == null || !o.IsTargetable) return false;
            var postion = new Vector3(o.Position.X, o.Position.Y, o.Position.Z);
            var address = (GameObject*)(void*)o.Address;
            if (address->ObjectKind != ObjectKind.EventObj) return false;
            if (address->EventId.ContentId != EventHandlerType.TreasureHuntDirector) return false;
            return true;
        });

        if (portal == null)
        {
            gameObject = null;
            return false;
        }
        gameObject = portal;
        return true;
    }

    /// <summary>
    /// Attempts to find a generic event object in the current area.
    /// </summary>
    /// <param name="gameObject">The event object, if found; otherwise, null.</param>
    /// <returns>True if an event object is found, otherwise false.</returns>
    public unsafe static bool FindObj(out IGameObject gameObject)
    {
        var portal = Svc.Objects.FirstOrDefault(o =>
        {
            if (o == null || !o.IsTargetable) return false;
            var postion = new Vector3(o.Position.X, o.Position.Y, o.Position.Z);
            var address = (GameObject*)(void*)o.Address;
            if (address->ObjectKind != ObjectKind.EventObj) return false;
            if (address->SubKind != 0) return false;
            if (address->NamePlateIconId != 0) return false;
            if (address->EventId.ContentId != 0) return false;
            return true;
        });

        if (portal == null)
        {
            gameObject = null;
            return false;
        }
        gameObject = portal;
        return true;
    }

    /// <summary>
    /// Attempts to find a random generic event object in the current area.
    /// </summary>
    /// <param name="gameObject">A random event object, if found; otherwise, null.</param>
    /// <returns>True if an event object is found, otherwise false.</returns>
    public unsafe static bool FindObjRandom(out IGameObject gameObject)
    {
        var gateExit = Svc.Objects.Where(o =>
        {
            if (o == null || !o.IsTargetable) return false;
            var position = new Vector3(o.Position.X, o.Position.Y, o.Position.Z);
            var address = (GameObject*)(void*)o.Address;
            if (Math.Abs(Svc.ClientState.LocalPlayer.Position.Y - position.Y) > 30) return false;
            if (address->ObjectKind != ObjectKind.EventObj) return false;
            if (address->SubKind != 0) return false;
            if (address->NamePlateIconId != 0) return false;
            if (address->EventId.ContentId != 0) return false;
            return true;
        }).ToArray();

        if (gateExit.Length == 0)
        {
            gameObject = null;
            return false;
        }
        gameObject = gateExit.GetRandom();
        return true;
    }
}
