using Dalamud.Interface.Components;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasureMaps.Helpers;
using TreasureMaps.IPC;

namespace TreasureMaps.UI.MainWindow.StartTabUI;

public class DrawVnavPluginConfigConfig
{
    public static void Draw(ref bool goToTreasure, ref bool digMap, ref bool openCoffer, ref bool enterPortal, ref bool doDungeon)
    {
        bool hasVnav = Generic.IsPluginInstalled("vnavmesh");

        ImGui.BeginDisabled(!hasVnav);
        if (ImGui.Checkbox("Go to Treasure Location", ref goToTreasure))
        {
            HandleVnavConfig(hasVnav, ref goToTreasure);
        }
        ImGui.EndDisabled();

        if (hasVnav)
        {
            ImGuiComponents.HelpMarker("After teleporting, fly to treasure coffer location");
        }
        else
        {
            ImGui.SameLine();
            Generic.CheckMarkTipString(hasVnav, "vnavmesh", NavmeshIPC.Repo);
            ImGui.NewLine();
        }

        if (goToTreasure)
        {

            DrawTreasureSettings(ref digMap, ref openCoffer, ref enterPortal);
        }

        ImGui.BeginDisabled(!hasVnav);
        if (ImGui.Checkbox("Do Treasure Dungeons", ref goToTreasure))
        {
            HandleDungeon(hasVnav, ref doDungeon);
        }
        ImGui.EndDisabled();

        if (hasVnav)
        {
            ImGuiComponents.HelpMarker("After entering a treasure dungeon, auto complete the dungeon\n" +
                                       "IT IS RECOMMENDED HAVING AN AUTOROTATION AND AI PLUGIN TO AUTOMATE");
        }
        else
        {
            ImGui.SameLine();
            Generic.CheckMarkTipString(hasVnav, "vnavmesh", NavmeshIPC.Repo);
            ImGui.NewLine();
        }
    }

    private static void DrawTreasureSettings(ref bool digMap, ref bool openCoffer, ref bool enterPortal)
    {
        ImGui.Indent(10f);

        if (ImGui.Checkbox("Dig Map", ref digMap))
        {
            C.digMap = digMap;
            C.Save();
        }
        ImGuiComponents.HelpMarker("Digs map once arriving at treasure location");

        ImGui.BeginDisabled(!digMap);
        if (ImGui.Checkbox("Open Coffer", ref openCoffer))
        {
            HandleOpenCoffer(digMap, ref openCoffer);
        }
        ImGui.EndDisabled();
        ImGuiComponents.HelpMarker("Opens coffer after digging map\n\"Dig Map\" must also be enabled");

        ImGui.BeginDisabled(!digMap);
        if (ImGui.Checkbox("Enter Portal", ref enterPortal))
        {
            HandlePortal(digMap, ref enterPortal);
        }
        ImGui.EndDisabled();
        ImGuiComponents.HelpMarker("Enters portal if it spawns from map\n\"Dig Map\" must also be enabled");

        ImGui.Unindent(10f);
    }

    private static void HandleVnavConfig(bool hasVnav, ref bool goToTreasure)
    {
        if (!hasVnav)
        {
            goToTreasure = false;
            C.goToTreasure = false;
        }
        else
        {
            C.goToTreasure = goToTreasure;
        }
        C.Save();
    }

    private static void HandleOpenCoffer(bool digMap, ref bool openCoffer)
    {
        if (!digMap)
        {
            openCoffer = false;
            C.openCoffer = false;
        }
        else
        {
            C.openCoffer = openCoffer;
        }
        C.Save();
    }

    private static void HandlePortal(bool digMap, ref bool enterPortal)
    {
        if (!digMap)
        {
            enterPortal = false;
            C.enterPortal = false;
        }
        else
        {
            C.enterPortal = enterPortal;
        }
        C.Save();
    }

    private static void HandleDungeon(bool hasVnav, ref bool doDungeon)
    {
        if (!hasVnav)
        {
            doDungeon = false;
            C.doDungeon = false;
        }
        else
        {
            C.doDungeon = doDungeon;
        }
        C.Save();
    }
}
