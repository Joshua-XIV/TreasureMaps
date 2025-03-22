using Dalamud.Interface.Components;
using ImGuiNET;
using TreasureMaps.Helpers;

namespace TreasureMaps.UI.MainWindow.StartTabUI;

public class DrawRotationPluginConfig
{
    public static void Draw(ref bool autoRotaion)
    {
        bool hasRSR = Generic.IsPluginInstalled("RotationSolver");
        ImGui.BeginDisabled(!hasRSR);
        if (ImGui.Checkbox("Auto Manage Rotation Plugin", ref autoRotaion))
        {
            HandleRotationPlugin(hasRSR, ref autoRotaion);
        }
        ImGui.EndDisabled();

        if (hasRSR)
        {
            ImGuiComponents.HelpMarker("This plugin will enable the following Rotation Plugins\n" +
                                       "* Currently supported: Rotation Solver\n" +
                                       "**Soon to be supported: Wrath and BossMod AutoRotation");
        }
        else
        {
            ImGui.SameLine();
            Generic.CheckMarkTipString(hasRSR, "Rotation Solver", rotationSolverRepo);
            ImGui.NewLine();
        }
    }

    private static void HandleRotationPlugin(bool hasRSR, ref bool autoRotaion)
    {
        if (!hasRSR)
        {
            autoRotaion = false;
            C.autoRotaion = false;
        }
        else
        {
            C.autoRotaion = autoRotaion;
        }
        C.Save();
    }
}