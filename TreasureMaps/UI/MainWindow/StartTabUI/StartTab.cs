using ImGuiNET;
using TreasureMaps.Helpers;
using TreasureMaps.Scheduler;
using System.Numerics;
using Dalamud.Interface.Components;
using Dalamud.Interface.Colors;
using TreasureMaps.IPC;

namespace TreasureMaps.UI.MainWindow.StartTabUI;

internal class StartTab
{

    private static string currentItem = C.mapSelected;
    private static string searchQuery = string.Empty;
    private static bool specificMap = C.specificMap;
    private static bool goToTreasure = C.goToTreasure;
    private static bool digMap = C.digMap;
    private static bool openCoffer = C.openCoffer;
    private static bool enterPortal = C.enterPortal;
    private static bool autoRotaion = C.autoRotaion;
    private static bool bossModReborn = C.bossModRebornPlugin;
    private static bool doDungeon = C.doDungeon;

    public unsafe static void Draw()
    {
        bool hasRSR = Generic.IsPluginInstalled("RotationSolver");
        bool hasVnav = Generic.IsPluginInstalled("vnavmesh");
        DrawRotationPluginConfig.Draw(ref autoRotaion);
        DrawVnavPluginConfigConfig.Draw(ref goToTreasure, ref digMap, ref openCoffer, ref enterPortal, ref doDungeon);

        if (ImGui.Checkbox("Use BossModReborn AI", ref bossModReborn))
        {
            C.bossModRebornPlugin = bossModReborn;
            C.Save();
        }

        if (ImGui.Checkbox("Select Specific Map", ref specificMap))
        {
            C.specificMap = specificMap;
            C.Save();
        }
        
        ImGuiComponents.HelpMarker("Select any map of your choosing if wanted.\n" +
                                   "By Default any map will be chosen in your inventory to decipher\n");

        if (!specificMap)
        {
            ImGui.Text($"Total Map Count: {Inventory.GetTotalMapCount()}");
        }

        var combinedList = new Dictionary<uint, string>
        {
            { (uint)Inventory.GetTotalMapCount(), "Default" }
        };
        combinedList.AddRange(TreasureMapIds.Select(map => new KeyValuePair<uint, string>(map.Key, map.Value)));

        if (specificMap)
        {
            if (ImGui.InputText("Search", ref searchQuery, 100)) { }
            ImGui.Text($"Selected Map: \t {currentItem}");
            if (currentItem == "Default")
            {
                ImGui.Text($"Total Selected Count:  \t{Inventory.GetTotalMapCount()}");
            }
            else
            {
                ImGui.Text($"Total Selected Count:  \t{Inventory.GetMapCount(TreasureMapIds.FindKeysByValue(currentItem).FirstOrDefault())}");
            }

            // Check if the warning is visible
            bool showWarning = !SchedulerMain.runPlugin && Inventory.IsMapDeciphered() && !C.disableWarning;

            // Calculate dynamic height for the map list
            float availableHeight = ImGui.GetContentRegionAvail().Y - (showWarning ? 60 : 30); // Subtract 60px if warning, else 30px to reserve space at bottom

            // Ensure a minimum height so the list is always scrollable
            float childHeight = Math.Max(availableHeight, 50);

            // Start a new window to display the list
            ImGui.BeginChild("MapList", new Vector2(0, childHeight), true, ImGuiWindowFlags.AlwaysVerticalScrollbar);

            foreach (var map in combinedList)
            {
                // Filter the list based on the search query
                if (!string.IsNullOrEmpty(searchQuery) && !map.Value.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip this item if it doesn't match the search query
                }

                bool is_selected_map = currentItem == map.Value;
                if (ImGui.Selectable(map.Value, is_selected_map))
                {
                    currentItem = map.Value;
                    C.mapSelected = map.Value;
                    C.Save();
                }

                if (is_selected_map && ImGui.IsItemHovered())
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndChild();
        }

        if (!SchedulerMain.runPlugin && Inventory.IsMapDeciphered() && !C.disableWarning)
        {
            ImGui.TextColored(ImGuiColors.DalamudYellow , "Warning! A map is currently deciphered!");
            ImGuiComponents.HelpMarker("Map that is currently deciphered will be ran first\nTo disable warnings, go to settings and check \"Disable Warnings\"");
        }

        if (ImGui.Button(!SchedulerMain.runPlugin ? "Start Maps" : "Stop Maps", new Vector2(ImGui.GetContentRegionAvail().X, 30)))
        {
            if (!SchedulerMain.runPlugin)
            {
                SchedulerMain.EnablePlugin();
            }
            else
            {
                SchedulerMain.DisablePlugin();
            }
        }
    }
}
