using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Numerics;
using Dalamud.Interface;
using TreasureMaps.Helpers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using TreasureMaps.UI.DisclaimerWindow;
using TreasureMaps.UI.MainWindow.DebugTabUI;
using TreasureMaps.UI.MainWindow.SettingsTabUI;
using TreasureMaps.UI.MainWindow.StartTabUI;

namespace TreasureMaps.UI.MainWindow;

internal class MainWindow : Window
{
    public MainWindow() : base($"Treasure Maps {P.GetType().Assembly.GetName().Version} ###TreasureMapsMainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(400, 400), 
            MaximumSize = new(9999,9999)
        };

        P.windowSystem.AddWindow(this);
    }

    public override void Draw()
    {
        if (!C.acceptedDisclaimer)
        {
            Disclaimer.Draw();
        }
        else
        {
            if (C.DEBUG)
                ImGuiEx.EzTabBar
                    ("Maps Bar",
                    ("Start", StartTab.Draw, null, true),
                    ("More Config", SettingsTab.Draw, null, true),
                    ("Debug", DebugTab.Draw, null, true));
            else
                ImGuiEx.EzTabBar
                    ("Maps Bar",
                    ("Start", StartTab.Draw, null, true),
                    ("More Config", SettingsTab.Draw, null, true));
        }
    }

    public void Dispose() {}
}