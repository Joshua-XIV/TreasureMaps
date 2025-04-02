using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasureMaps.Helpers;
using TreasureMaps.Scheduler.Tasks;
using TreasureMaps.Toasts;

namespace TreasureMaps.UI.MainWindow.DebugTabUI;

internal class DebugTab
{
    
    private static string txt = "";
    private static string txt2 = "";
    private static string txt3 = "";
    private static string txt4 = "";
    private static string txt5 = "";
    private static string txt6 = "";
    private static string txt7 = "";
    private static string txt8 = "";
    private static string txt9 = "";
    private static string lastToast = "";
    private static string key = "";
    private static string toDoText = "";
    private static string toDoSubText = "waiting... ";
    public unsafe static void Draw()
    {
        // ArcaneSphere subkind = 0
        // ArcaneSphere objectkind = 0
        //
        IGameObject? gameObject;
        if (ImGui.Button("Test Target"))
        {
            gameObject = Svc.Targets.Target;
            var address = (GameObject*)(void*)gameObject.Address;
            var objectkind = ObjectKind.BattleNpc;
            if (gameObject != null)
            {
                txt = $"Object Name = {address->NameString.ToString()}";
                txt2 = $"Object Kind = {(byte)address->ObjectKind}";
                txt3 = $"Sub Kind = {address->SubKind.ToString()}";
                txt4 = $"Nameplate Icon ID = {address->NamePlateIconId.ToString()}";
                txt5 = $"Content Id = {(ushort)address->EventId.ContentId}";
                txt6 = $"Entity Id = {address->EntityId}";
                txt7 = $"Base Id = {address->BaseId}";
                txt8 = $"Owner Id = {address->OwnerId}";
                txt9 = $"Game Object Id = {address->GetGameObjectId().ObjectId}";

                var Event = EventHandlerContent.TreasureHuntDirector;
            }
        }
        ImGui.Text(txt);
        ImGui.Text(txt2);
        ImGui.Text(txt3);
        ImGui.Text(txt4);
        ImGui.Text(txt5);
        ImGui.Text(txt6);
        ImGui.Text(txt7);
        ImGui.Text(txt8);
        ImGui.Text(txt9);

        if (ImGui.Button("Get Last Toast"))
        {
            if (P.toast.GetLastToast() != null)
                lastToast = P.toast.GetLastToast();
        }
        ImGui.Text(lastToast);

        var pointFloor = P.navmesh.PointOnFloor(new(Zones.FlagXCoords(), 1024, Zones.FlagYCoords()), true, 1f);

        if (pointFloor.HasValue)
        {
            ImGui.Text($"Distance to Flag: {Distance.GetDistanceToPlayer(pointFloor.Value)}");
            ImGui.Text($"X-Flag: {pointFloor.Value.X}");
            ImGui.Text($"Y-Flag: {pointFloor.Value.Y}");
            ImGui.Text($"Z-Flag: {pointFloor.Value.Z}");
            if (Svc.Targets.Target != null)
                ImGui.Text($"Distance to Target: {Distance.GetDistanceToPlayer(Svc.Targets.Target)}");
        }
        else
        {
            ImGui.Text("No valid map point.");
        }

        if (ImGui.Button("Get last _ToDo text##text"))
        {
            toDoText = Generic.GetToDoText();
        }

        if (ImGui.Button("Get last _ToDo sub text##subtext"))
        {
            toDoSubText = Generic.GetToDoSubText();
        }

        if (ImGui.Button("Test Movement Key"))
        {
            TaskGoNextFloor.Enqueue();
        }

        ImGui.Text(toDoText);
        ImGui.Text(toDoSubText);
        InternalLog.PrintImgui();
    }
}
