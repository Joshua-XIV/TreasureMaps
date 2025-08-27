using Dalamud.Interface.Components;
using Dalamud.Plugin.Services;
using ECommons.ImGuiMethods;
using ImGuiNET;
using TreasureMaps.Helpers;

namespace TreasureMaps.UI.MainWindow.SettingsTabUI;

internal class SettingsTab
{
    private static bool selfRepair = C.selfRepair;
    private static float repairSlider = C.repairSlider;
    private static bool disableWarnings = C.disableWarning;
    private static bool DEBUG = C.DEBUG;
    public static void Draw()
    {
        if (ImGui.Checkbox("Self Repair", ref selfRepair))
        {
            C.selfRepair = selfRepair;
            C.Save();
        }
        ImGuiComponents.HelpMarker("Self repair between map deciphers");

        if (selfRepair)
        {
            ImGui.Indent(10f);
            ImGui.PushItemWidth(150f);
            if (ImGuiEx.SliderFloat("Repair Threshold ###Repair Slider", ref repairSlider, 0f, 100f, "%1.0f"))
            {
                if (repairSlider >= 100)
                {
                    C.repairSlider = 99.9f;
                }
                else
                {
                    C.repairSlider = repairSlider;
                }
                C.Save();
            }
            ImGuiComponents.HelpMarker("Condition percentage required to repair");
            ImGui.Unindent(10f);
        }

        if (ImGui.Checkbox("Disable Warnings", ref disableWarnings))
        {
            C.disableWarning = disableWarnings;
            C.Save();
        }

        if (ImGui.Checkbox("Enable Debug", ref DEBUG))
        {
            C.DEBUG = DEBUG;
            C.Save();
        }
    }
}
