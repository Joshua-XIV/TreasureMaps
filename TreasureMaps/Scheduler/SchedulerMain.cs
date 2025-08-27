using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Config;
using ECommons;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Numerics;
using TreasureMaps.Helpers;
using TreasureMaps.Scheduler.Tasks;

namespace TreasureMaps.Scheduler;

internal static class SchedulerMain
{
    private static bool startDungeon = true;
    internal static bool runPluginInternal;
    internal static bool runPlugin
    {
        get { return runPluginInternal; }
        private set { runPluginInternal = value; }
    }

    internal static bool EnablePlugin()
    {
        DuoLog.Information("Starting Plugin");
        runPlugin = true;
        if (C.autoRotaion)
            PluginManager.EnableAutoRotationPlugin();
        return true;
    }

    internal static bool DisablePlugin()
    {
        DuoLog.Information("Disabling Plugin");
        runPlugin = false;
        P.navmesh.Stop();
        P.taskManager.Abort();
        if (C.autoRotaion)
            PluginManager.DisableAutoRotationPlugin();
        if (C.bossModRebornPlugin)
            PluginManager.DisableBossMod();
        return true;
    }

    internal static void Tick()
    {
        /*
        if (runPlugin)
        {
            if (!P.taskManager.IsBusy)
            {
                if (Zones.IsInNormalTreasureDungeon())
                {
                    if (C.doDungeon)
                    {
                        if (startDungeon)
                        {
                            startDungeon = false;
                            var initialPoint = InitialTreasureDungeonPosition.GetValueOrDefault(Zones.CurrentZoneId());
                            P.taskManager.Enqueue(() => Statuses.PlayerNotBusy());
                            P.taskManager.Enqueue(() =>
                            {
                                if (Math.Abs(Svc.ClientState.LocalPlayer.Position.Y - initialPoint.Y) <= 30)
                                {
                                    TaskMoveTo.Enqueue(initialPoint, "Start");
                                    TaskGoNextFloor.Enqueue();
                                    P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.Jumping61] && Statuses.PlayerNotBusy());
                                }
                            });
                        }
                        if (!startDungeon)
                        {
                            if (Targeting.FindStartFloorChest(out var startFloor) && Statuses.PlayerNotBusy())
                            {
                                TaskMoveTo.Enqueue(startFloor.Position, startFloor.Name.ToString(), 1f);
                                TaskTarget.Enqueue(startFloor);
                                P.taskManager.Enqueue(() => !Movement.IsMoving());
                                TaskInteract.Enqueue(startFloor);
                                TaskSelectYes.Enqueue();
                                P.taskManager.Enqueue(() => !Targeting.FindStartFloorChest(out var tempStartFloor));
                                // Delay getting the subtext and retrieving it after the delay
                                P.taskManager.Enqueue(() =>
                                {
                                    P.taskManager.DelayNext(1000);
                                    Generic.PluginLogInfo("SubText");
                                    TaskCombatMapDungeon.Enqueue(Generic.GetToDoSubText());
                                    P.taskManager.Enqueue(() => Statuses.PlayerNotBusy());
                                });
                            }
                            else if (Targeting.FindAllLoot(out var loots) && Statuses.PlayerNotBusy())
                            {
                                Generic.PluginLogInfo("Looking for Loot");
                                foreach (var loot in loots)
                                {
                                    TaskMoveTo.Enqueue(loot.Position, loot.Name.ToString(), 1f);
                                    TaskTarget.Enqueue(loot);
                                    P.taskManager.Enqueue(() => !Movement.IsMoving());
                                    TaskInteract.Enqueue(loot);
                                }
                                P.taskManager.Enqueue(() => Generic.PluginLogInfo("Loot Test"));
                                P.taskManager.Enqueue(() => P.taskManager.Enqueue(() => Svc.Targets.Target == null && !Targeting.FindAllLoot(out var tempLoot)));
                            }
                            else if (Targeting.FindObjRandom(out var gatesAndExit) && gatesAndExit != null && Statuses.PlayerNotBusy() && !Targeting.TryGetAnyClosestEnemy(out var tempEnemy))
                            {
                                TaskMoveTo.Enqueue(gatesAndExit.Position, gatesAndExit.Name.ToString(), 1f);
                                TaskTarget.Enqueue(gatesAndExit);
                                P.taskManager.Enqueue(() => !Movement.IsMoving());
                                TaskInteract.Enqueue(gatesAndExit);
                                TaskSelectYes.Enqueue();
                                P.taskManager.Enqueue(() => Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent] || !Zones.IsInNormalTreasureDungeon());
                                Generic.PluginLogInfo("Status Busy");
                                P.taskManager.Enqueue(() => Statuses.PlayerNotBusy(), 1000 * 60 * 1);
                                Generic.PluginLogInfo("Status Free");
                                P.taskManager.Enqueue(() =>
                                {
                                    if (Statuses.PlayerNotBusy() && Zones.IsInNormalTreasureDungeon() && !Targeting.FindObjRandom(out var temp))
                                    {
                                        TaskGoNextFloor.Enqueue();
                                        P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.Jumping61] && Statuses.PlayerNotBusy());
                                    }
                                });
                            }
                            else if (Targeting.TryGetAnyClosestEnemy(out var enemy) && Statuses.PlayerNotBusy())
                            {
                                TaskCombatMapDungeon.Enqueue();
                            }
                            else
                            {
                                if (C.DEBUG)
                                {
                                    Generic.PluginLogInfo("No Condition was met");
                                    P.taskManager.DelayNext(1000);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (C.DEBUG)
                        {
                            Generic.PluginLogInfo("C.doDungeon is false");
                            P.taskManager.DelayNext(1000);
                        }
                    }
                }
                else if (Zones.IsInShiftingTreasureDungeon())
                {
                    if (C.doDungeon)
                    {
                        P.taskManager.Enqueue(() => Statuses.PlayerNotBusy(), 1000 * 60, false);
                        if (Targeting.TryGetAnyClosestEnemy(out var enemy) && enemy != null)
                        {
                            TaskCombatMapDungeon.Enqueue();
                            Generic.PluginLogInfo("Combat Done");
                            P.taskManager.DelayNext(1000);
                        }
                        else if (Targeting.FindAllLoot(out var loots) && Statuses.PlayerNotBusy())
                        {
                            foreach (var loot in loots)
                            {
                                TaskMoveTo.Enqueue(loot.Position, loot.Name.ToString(), 1f);
                                TaskTarget.Enqueue(loot);
                                P.taskManager.Enqueue(() => !Movement.IsMoving());
                                TaskInteract.Enqueue(loot);
                            }
                            P.taskManager.Enqueue(() => Generic.PluginLogInfo("Loot Test"));
                            P.taskManager.Enqueue(() => P.taskManager.Enqueue(() => Svc.Targets.Target == null && !Targeting.FindAllLoot(out var temploot)));
                        }
                        else if (Targeting.FindObj(out var sphere))
                        {
                            if (sphere != null && sphere.IsTargetable)
                            {
                                TaskMoveTo.Enqueue(sphere.Position, sphere.Name.ToString(), 1f);
                                TaskTarget.Enqueue(sphere);
                                P.taskManager.Enqueue(() => !Movement.IsMoving());
                                TaskInteract.Enqueue(sphere);
                                TaskSelectYes.Enqueue();
                                P.taskManager.Enqueue(() => Svc.Condition[ConditionFlag.Occupied38] || !Zones.IsInShiftingTreasureDungeon() || Svc.Condition[ConditionFlag.BetweenAreas]);
                                P.taskManager.Enqueue(() =>
                                {
                                    if (Svc.Condition[ConditionFlag.BetweenAreas])
                                    {
                                        P.taskManager.Enqueue(() => Statuses.PlayerNotBusy() && !Zones.IsInShiftingTreasureDungeon(), 1000 * 60);
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        if (C.DEBUG)
                        {
                            Generic.PluginLogInfo("C.doDungeon is false");
                            P.taskManager.DelayNext(1000);
                        }
                    }
                }
                else if (Svc.Condition[ConditionFlag.BoundByDuty] && Statuses.PlayerNotBusy() && Targeting.FindPortal(out var portal))
                {
                    startDungeon = true;
                    if (LootWindow.hasLootNotifitcation())
                    {
                        P.taskManager.Enqueue(() => !LootWindow.hasLootNotifitcation());
                    }
                    else if (C.enterPortal && C.digMap)
                    {
                        TaskTarget.Enqueue(portal);
                        if (Svc.Condition[ConditionFlag.Diving])
                        {
                            P.taskManager.Enqueue(() => Chat.Instance.ExecuteCommand($"/vnav flyto {portal.Position.X} {portal.Position.Y} {portal.Position.Z}"));
                            TaskFlyTo.Enqueue("Portal");
                        }
                        else
                            TaskMoveTo.Enqueue(portal.Position, portal.Name.ToString());
                        P.taskManager.Enqueue(() => !Movement.IsMoving());
                        TaskInteract.Enqueue(portal);
                        TaskSelectYes.Enqueue();
                        P.taskManager.Enqueue(() => Zones.IsInNormalTreasureDungeon() || Zones.IsInShiftingTreasureDungeon());
                    }
                    else
                    {
                        if (C.DEBUG)
                        {
                            Generic.PluginLogInfo("C.enterPortal is false");
                            P.taskManager.DelayNext(1000);
                        }
                    }
                }
                else if (Svc.Condition[ConditionFlag.BoundByDuty] && Statuses.PlayerNotBusy() && Targeting.FindChest(out var treasureChest) && treasureChest != null)
                {
                    if (TreasureHuntExamineCofferText.Contains(Generic.GetToDoText()))
                    {
                        if (C.openCoffer && C.digMap)
                        {
                            TaskTarget.Enqueue(treasureChest);
                            TaskMoveTo.Enqueue(treasureChest.Position, treasureChest.Name.ToString());
                            TaskInteract.Enqueue(treasureChest);
                            TaskStartTreasureHunt.Enqueue();
                            P.taskManager.Enqueue(() => !TreasureHuntExamineCofferText.Contains(Generic.GetToDoText()), 1000);
                        }
                    }
                    else if (!TreasureHuntExamineCofferText.Contains(Generic.GetToDoText()) && C.autoRotaion)
                    {
                        P.taskManager.Enqueue(() => Statuses.InCombat());
                        TaskDoCombatUntilToast.Enqueue();
                        TaskCombat.Enqueue();
                        TaskTarget.Enqueue(treasureChest);
                        TaskMoveTo.Enqueue(treasureChest.Position, treasureChest.Name.ToString());
                        TaskInteract.Enqueue(treasureChest);
                        P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.BoundByDuty] || Targeting.FindPortal(out var tempPortal));
                    }
                    else
                    {
                        if (C.DEBUG)
                        {
                            if (!C.openCoffer)
                                Generic.PluginLogInfo("C.opencoffer is false");
                            if (!C.autoRotaion)
                                Generic.PluginLogInfo("C.autorotation is false");
                        }
                        P.taskManager.DelayNext(1000);
                    }    
                }
                else if (Actions.NeedsRepair(C.repairSlider) && C.selfRepair)
                {
                    TaskSelfRepair.Enqueue();
                }
                else if (Inventory.IsMapDeciphered() && Zones.IsInZone(Zones.FlagZoneID()) && Generic.IsPluginInstalled("vnavmesh"))
                {
                    if (P.navmesh.IsReady())
                    {
                        if (C.goToTreasure)
                        {
                            var pointFloor = P.navmesh.PointOnFloor(new(Zones.FlagXCoords(), 1024, Zones.FlagYCoords()), false, 1f);
                            if (Inventory.GetMapDecipheredId() == 2002260 || Inventory.GetMapDecipheredId() == 2002386)
                            {
                                pointFloor = Distance.FindClosestPortal(pointFloor.Value);
                            }
                            if (Distance.GetDistanceToPlayer(pointFloor) >= 20)
                            {
                                TaskMount.Enqueue();
                                TaskJumpFly.Enqueue();
                                P.taskManager.Enqueue(() => ECommons.Automation.Chat.Instance.ExecuteCommand($"/vnav flyto {pointFloor.Value.X} {pointFloor.Value.Y} {pointFloor.Value.Z}"));
                                TaskFlyTo.Enqueue("Flag");
                                TaskDisMount.Enqueue();
                            }
                            else if (Distance.GetDistanceToPlayer(pointFloor) <= 20 && Svc.Condition[ConditionFlag.InFlight] && Inventory.GetMapDecipheredId() != 2002260 && Inventory.GetMapDecipheredId() != 2002386)
                            {
                                P.taskManager.Enqueue(() => P.navmesh.Stop());
                                TaskDisMount.Enqueue();
                            }
                            else if (C.digMap && C.goToTreasure)
                            {
                                P.taskManager.Enqueue(() => P.navmesh.Stop());
                                TaskDigMap.Enqueue();
                            }
                            else
                            {
                                Generic.PluginLogInfo("C.digmap is false");
                            }
                        }
                        else
                        {
                            if (C.DEBUG)
                            {
                                Generic.PluginLogInfo("Player reached Treasure Location or C.goToTreasure is false");
                            }
                            P.taskManager.DelayNext(1000);
                        }
                    }
                    else
                    {
                        if (C.DEBUG)
                        {
                            Generic.PluginLogInfo("Vnav not ready");
                            P.taskManager.DelayNext(1000);
                        }
                        TaskNavIsReady.Enqueue();
                    }
                }
                else if (Inventory.IsMapDeciphered() && !Zones.IsInZone(Zones.FlagZoneID()))
                {
                    TaskOpenDecipheredMap.Enqueue(Inventory.GetMapDecipheredId());
                    TaskTeleport.Enqueue(Zones.GetClosestAetheryte(), Zones.FlagZoneID());
                }
                else if (!Inventory.IsMapDeciphered() && Inventory.HasMap())
                {
                    TaskDecipherMap.Enqueue();
                    P.taskManager.Enqueue(() => Inventory.IsMapDeciphered());
                }
                else if (!Inventory.IsMapDeciphered() && !Inventory.HasMap())
                {
                    Generic.AllWarningEnqueue("No Maps In Invetory");
                    DisablePlugin();
                }
            }
        }*/

        if (!runPlugin) return;
        if (P.taskManager.IsBusy) return; 

        if (Zones.IsInNormalTreasureDungeon())
        {
            DoTreasureDungeon();
        }
        else if (Zones.IsInShiftingTreasureDungeon())
        {
            DoShiftingDungeon();
        }
        else if (Svc.Condition[ConditionFlag.BoundByDuty] && Statuses.PlayerNotBusy() && Targeting.FindPortal(out var portal))
        {
            EnterPortal(portal);
        }
        else if (Svc.Condition[ConditionFlag.BoundByDuty] && Statuses.PlayerNotBusy() && Targeting.FindChest(out var treasureChest) && treasureChest != null)
        {
            DoTreasureHunt(treasureChest);
        }
        else if (Actions.NeedsRepair(C.repairSlider) && C.selfRepair)
        {
            TaskSelfRepair.Enqueue();
        }
        else if (Inventory.IsMapDeciphered() && Zones.IsInZone(Zones.FlagZoneID()) && Generic.IsPluginInstalled("vnavmesh"))
        {
            GoToMap();
        }
        else if (Inventory.IsMapDeciphered() && !Zones.IsInZone(Zones.FlagZoneID()))
        {
            TaskOpenDecipheredMap.Enqueue(Inventory.GetMapDecipheredId());
            TaskTeleport.Enqueue(Zones.GetClosestAetheryte(), Zones.FlagZoneID());
        }
        else if (!Inventory.IsMapDeciphered() && Inventory.HasMap())
        {
            TaskDecipherMap.Enqueue();
            P.taskManager.Enqueue(() => Inventory.IsMapDeciphered());
        }
        else if (!Inventory.IsMapDeciphered() && !Inventory.HasMap())
        {
            Generic.AllWarningEnqueue("No Maps In Invetory");
            DisablePlugin();
        }
    }

    internal static void DoTreasureDungeon()
    {
        if (C.doDungeon)
        {
            if (startDungeon)
            {
                startDungeon = false;
                var initialPoint = InitialTreasureDungeonPosition.GetValueOrDefault(Zones.CurrentZoneId());
                P.taskManager.Enqueue(() => Statuses.PlayerNotBusy());
                P.taskManager.Enqueue(() =>
                {
                    if (Math.Abs(Svc.ClientState.LocalPlayer.Position.Y - initialPoint.Y) <= 30)
                    {
                        TaskMoveTo.Enqueue(initialPoint, "Start");
                        TaskGoNextFloor.Enqueue();
                        P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.Jumping61] && Statuses.PlayerNotBusy());
                    }
                });
            }
            if (!startDungeon)
            {
                if (Targeting.FindStartFloorChest(out var startFloor) && Statuses.PlayerNotBusy())
                {
                    TaskMoveTo.Enqueue(startFloor.Position, startFloor.Name.ToString(), 1f);
                    TaskTarget.Enqueue(startFloor);
                    P.taskManager.Enqueue(() => !Movement.IsMoving());
                    TaskInteract.Enqueue(startFloor);
                    TaskSelectYes.Enqueue();
                    P.taskManager.Enqueue(() => !Targeting.FindStartFloorChest(out var tempStartFloor));
                    // Delay getting the subtext and retrieving it after the delay
                    P.taskManager.Enqueue(() =>
                    {
                        P.taskManager.DelayNext(1000);
                        Generic.PluginLogInfo("SubText");
                        TaskCombatMapDungeon.Enqueue(Generic.GetToDoSubText());
                        P.taskManager.Enqueue(() => Statuses.PlayerNotBusy());
                    });
                }
                else if (Targeting.FindAllLoot(out var loots) && Statuses.PlayerNotBusy())
                {
                    Generic.PluginLogInfo("Looking for Loot");
                    foreach (var loot in loots)
                    {
                        TaskMoveTo.Enqueue(loot.Position, loot.Name.ToString(), 1f);
                        TaskTarget.Enqueue(loot);
                        P.taskManager.Enqueue(() => !Movement.IsMoving());
                        TaskInteract.Enqueue(loot);
                    }
                    P.taskManager.Enqueue(() => Generic.PluginLogInfo("Loot Test"));
                    P.taskManager.Enqueue(() => P.taskManager.Enqueue(() => Svc.Targets.Target == null && !Targeting.FindAllLoot(out var tempLoot)));
                }
                else if (Targeting.FindObjRandom(out var gatesAndExit) && gatesAndExit != null && Statuses.PlayerNotBusy() && !Targeting.TryGetAnyClosestEnemy(out var tempEnemy))
                {
                    TaskMoveTo.Enqueue(gatesAndExit.Position, gatesAndExit.Name.ToString(), 1f);
                    TaskTarget.Enqueue(gatesAndExit);
                    P.taskManager.Enqueue(() => !Movement.IsMoving());
                    TaskInteract.Enqueue(gatesAndExit);
                    TaskSelectYes.Enqueue();
                    P.taskManager.Enqueue(() => Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent] || !Zones.IsInNormalTreasureDungeon());
                    Generic.PluginLogInfo("Status Busy");
                    P.taskManager.Enqueue(() => Statuses.PlayerNotBusy(), 1000 * 60 * 1);
                    Generic.PluginLogInfo("Status Free");
                    P.taskManager.Enqueue(() =>
                    {
                        if (Statuses.PlayerNotBusy() && Zones.IsInNormalTreasureDungeon() && !Targeting.FindObjRandom(out var temp))
                        {
                            TaskGoNextFloor.Enqueue();
                            P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.Jumping61] && Statuses.PlayerNotBusy());
                        }
                    });
                }
                else if (Targeting.TryGetAnyClosestEnemy(out var enemy) && Statuses.PlayerNotBusy())
                {
                    TaskCombatMapDungeon.Enqueue();
                }
                else
                {
                    if (C.DEBUG)
                    {
                        Generic.PluginLogInfo("No Condition was met");
                        P.taskManager.DelayNext(1000);
                    }
                }
            }
        }
        else
        {
            Generic.PluginDebugInfo("C.doDungeon is false");
            P.taskManager.DelayNext(1000);
        }
    }

    internal static void DoShiftingDungeon()
    {
        if (C.doDungeon)
        {
            P.taskManager.Enqueue(() => Statuses.PlayerNotBusy(), 1000 * 60, false);
            if (Targeting.TryGetAnyClosestEnemy(out var enemy) && enemy != null)
            {
                TaskCombatMapDungeon.Enqueue();
                Generic.PluginLogInfo("Combat Done");
                P.taskManager.DelayNext(1000);
            }
            else if (Targeting.FindAllLoot(out var loots) && Statuses.PlayerNotBusy())
            {
                foreach (var loot in loots)
                {
                    TaskMoveTo.Enqueue(loot.Position, loot.Name.ToString(), 1f);
                    TaskTarget.Enqueue(loot);
                    P.taskManager.Enqueue(() => !Movement.IsMoving());
                    TaskInteract.Enqueue(loot);
                }
                P.taskManager.Enqueue(() => Generic.PluginLogInfo("Loot Test"));
                P.taskManager.Enqueue(() => P.taskManager.Enqueue(() => Svc.Targets.Target == null && !Targeting.FindAllLoot(out var temploot)));
            }
            else if (Targeting.FindObj(out var sphere))
            {
                if (sphere != null && sphere.IsTargetable)
                {
                    TaskMoveTo.Enqueue(sphere.Position, sphere.Name.ToString(), 1f);
                    TaskTarget.Enqueue(sphere);
                    P.taskManager.Enqueue(() => !Movement.IsMoving());
                    TaskInteract.Enqueue(sphere);
                    TaskSelectYes.Enqueue();
                    P.taskManager.Enqueue(() => Svc.Condition[ConditionFlag.Occupied38] || !Zones.IsInShiftingTreasureDungeon() || Svc.Condition[ConditionFlag.BetweenAreas]);
                    P.taskManager.Enqueue(() =>
                    {
                        if (Svc.Condition[ConditionFlag.BetweenAreas])
                        {
                            P.taskManager.Enqueue(() => Statuses.PlayerNotBusy() && !Zones.IsInShiftingTreasureDungeon(), 1000 * 60);
                        }
                    });
                }
            }
        }
        else
        {
            Generic.PluginDebugInfo("C.doDungeon is false");
            P.taskManager.DelayNext(1000);
        }
    }

    internal static void EnterPortal(IGameObject portal)
    {
        startDungeon = true;
        if (LootWindow.hasLootNotifitcation())
        {
            P.taskManager.Enqueue(() => !LootWindow.hasLootNotifitcation());
        }
        else if (C.enterPortal && C.digMap)
        {
            TaskTarget.Enqueue(portal);
            if (Svc.Condition[ConditionFlag.Diving])
            {
                P.taskManager.Enqueue(() => Chat.Instance.ExecuteCommand($"/vnav flyto {portal.Position.X} {portal.Position.Y} {portal.Position.Z}"));
                TaskFlyTo.Enqueue("Portal");
            }
            else
            {
                TaskMoveTo.Enqueue(portal.Position, portal.Name.ToString());
            }
            P.taskManager.Enqueue(() => !Movement.IsMoving());
            TaskInteract.Enqueue(portal);
            TaskSelectYes.Enqueue();
            P.taskManager.Enqueue(() => Zones.IsInNormalTreasureDungeon() || Zones.IsInShiftingTreasureDungeon());
        }
        else
        {
            Generic.PluginDebugInfo("C.enterPortal is false");
            P.taskManager.DelayNext(1000);
        }
    }

    internal static void DoTreasureHunt(IGameObject treasureChest)
    {
        if (TreasureHuntExamineCofferText.Contains(Generic.GetToDoText()))
        {
            if (C.openCoffer && C.digMap)
            {
                TaskTarget.Enqueue(treasureChest);
                TaskMoveTo.Enqueue(treasureChest.Position, treasureChest.Name.ToString());
                TaskInteract.Enqueue(treasureChest);
                TaskStartTreasureHunt.Enqueue();
                P.taskManager.Enqueue(() => !TreasureHuntExamineCofferText.Contains(Generic.GetToDoText()), 1000);
            }
        }
        else if (!TreasureHuntExamineCofferText.Contains(Generic.GetToDoText()) && C.autoRotaion)
        {
            P.taskManager.Enqueue(() => Statuses.InCombat());
            TaskDoCombatUntilToast.Enqueue();
            TaskCombat.Enqueue();
            TaskTarget.Enqueue(treasureChest);
            TaskMoveTo.Enqueue(treasureChest.Position, treasureChest.Name.ToString());
            TaskInteract.Enqueue(treasureChest);
            P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.BoundByDuty] || Targeting.FindPortal(out var tempPortal));
        }
        else
        {
            if (!C.openCoffer)
                Generic.PluginDebugInfo("C.opencoffer is false");
            if (!C.autoRotaion)
                Generic.PluginDebugInfo("C.autorotation is false");
            P.taskManager.DelayNext(1000);
        }
    }

    internal static void GoToMap()
    {
        if (P.navmesh.IsReady())
        {
            if (C.goToTreasure)
            {
                var pointFloor = P.navmesh.PointOnFloor(new(Zones.FlagXCoords(), 1024, Zones.FlagYCoords()), false, 1f);
                if (Inventory.GetMapDecipheredId() == 2002260 || Inventory.GetMapDecipheredId() == 2002386)
                {
                    pointFloor = Distance.FindClosestPortal(pointFloor.Value);
                }
                if (Distance.GetDistanceToPlayer(pointFloor) >= 20)
                {
                    TaskMount.Enqueue();
                    TaskJumpFly.Enqueue();
                    P.taskManager.Enqueue(() => ECommons.Automation.Chat.Instance.ExecuteCommand($"/vnav flyto {pointFloor.Value.X} {pointFloor.Value.Y} {pointFloor.Value.Z}"));
                    TaskFlyTo.Enqueue("Flag");
                    TaskDisMount.Enqueue();
                }
                else if (Distance.GetDistanceToPlayer(pointFloor) <= 20 && Svc.Condition[ConditionFlag.InFlight] && Inventory.GetMapDecipheredId() != 2002260 && Inventory.GetMapDecipheredId() != 2002386)
                {
                    P.taskManager.Enqueue(() => P.navmesh.Stop());
                    TaskDisMount.Enqueue();
                }
                else if (C.digMap && C.goToTreasure)
                {
                    P.taskManager.Enqueue(() => P.navmesh.Stop());
                    TaskDigMap.Enqueue();
                }
                else
                {
                    Generic.PluginLogInfo("C.digmap is false");
                }
            }
            else
            {
                Generic.PluginDebugInfo("Player reached Treasure Location or C.goToTreasure is false");
                P.taskManager.DelayNext(1000);
            }
        }
        else
        {
            Generic.PluginDebugInfo("Vnav not ready");
            P.taskManager.DelayNext(1000);
            TaskNavIsReady.Enqueue();
        }
    }
}

