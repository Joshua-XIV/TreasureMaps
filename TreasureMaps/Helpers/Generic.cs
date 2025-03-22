using Dalamud.Interface.Colors;
using ECommons.Automation;
using ECommons.ImGuiMethods;
using ECommons.Reflection;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace TreasureMaps.Helpers;

internal class Generic
{
    /// <summary>
    /// Sends a warning message through all logging locations, including in-game chat, Dalamud console log, and the plugin's debugging UI.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    internal static void AllWarningEnqueue(string message)
    {
        P.taskManager.Enqueue(() => PluginLog.Warning(message));
        P.taskManager.Enqueue(() => Notify.Warning(message));
        P.taskManager.Enqueue(() => DuoLog.Warning(message));
        P.taskManager.Enqueue(() => PluginLog.Debug(message));
    }

    /// <summary>
    /// Logs an informational message through Dalamud console log and the plugin's debugging log.
    /// </summary>
    /// <param name="message">The informational message to log.</param>
    internal static void PluginLogInfo(string message)
    {
        P.taskManager.Enqueue(() => PluginLog.Information(message));
        P.taskManager.Enqueue(() => PluginLog.Debug(message));
    }

    /// <summary>
    /// Logs a debug message through the plugin's debugging log.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    internal static void PluginDebugInfo(string message)
    {
        P.taskManager.Enqueue(() => PluginLog.Debug(message));
    }

    /// <summary>
    /// Checks if a specific plugin is installed in Dalamud.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to check.</param>
    /// <returns>True if the plugin is installed, otherwise false.</returns>
    internal static bool IsPluginInstalled(string pluginName)
    {
        return DalamudReflector.TryGetDalamudPlugin(pluginName, out _, false, true);
    }

    /// <summary>
    /// Displays colored text in ImGui based on whether a plugin is installed.
    /// </summary>
    /// <param name="PluginInstalled">Whether the plugin is installed.</param>
    /// <param name="text">The text to display.</param>
    internal static void IsPluginInstalledColorText(bool PluginInstalled, string text)
    {
        if (PluginInstalled)
            ImGui.TextColored(ImGuiColors.HealerGreen, $"- {text}");
        else
            ImGui.TextColored(ImGuiColors.DalamudRed, $"- {text}");
    }

    /// <summary>
    /// Displays a checkmark or cross in ImGui based on a boolean condition.
    /// </summary>
    /// <param name="enabled">The condition to evaluate.</param>
    internal static void CheckMarkTip(bool enabled)
    {
        if (!enabled)
        {
            FontAwesome.Print(ImGuiColors.DalamudRed, FontAwesome.Cross);
        }
        else if (enabled)
        {
            FontAwesome.Print(ImGuiColors.HealerGreen, FontAwesome.Check);
        }
    }

    /// <summary>
    /// Displays a checkmark or cross in ImGui based on whether a plugin is installed, along with a tooltip and click-to-copy functionality for a URL.
    /// </summary>
    /// <param name="PluginInstalled">Whether the plugin is installed.</param>
    /// <param name="Text">The text to display in the tooltip.</param>
    /// <param name="Url">The URL to copy when clicked.</param>
    internal static void CheckMarkTipString(bool PluginInstalled, string Text, string Url)
    {
        CheckMarkTip(PluginInstalled);
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("The following plugins are required to be installed and enabled: ");
            IsPluginInstalledColorText(PluginInstalled, Text);
            ImGui.Text("Click to Copy Repo URL");
            ImGui.EndTooltip();
            if (ImGui.IsItemClicked())
            {
                ImGui.SetClipboardText(Url);
                DuoLog.Information("Repo URL Copied");
                Notify.Info("Repo URL Copied");
            }
        }

        ImGui.SameLine();
    }

    /// <summary>
    /// Opens the character configuration menu for the currently logged-in character.
    /// </summary>
    /// <returns>True if the character configuration menu is open, otherwise false.</returns>
    internal unsafe static bool? OpenCharaSettings()
    {
        if (!IsOccupied())
        {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName("ConfigCharacter");
            if (addon != null && addon->IsVisible && addon->IsReady)
                return true;

            if (EzThrottler.Throttle("ConfigCharacterWait", 100))
                Chat.Instance.SendMessage("/characterconfig");
        }
        return false;
    }

    /// <summary>
    /// Opens the keybind configuration menu.
    /// </summary>
    /// <returns>True if the keybind configuration menu is open, otherwise false.</returns>
    internal unsafe static bool OpenKeyBindSettings()
    {
        TryGetAddonByName<AtkUnitBase>("ConfigKeybind", out var addon);
        if (addon == null)
        {
            if (EzThrottler.Throttle("Open"))
                Chat.Instance.SendMessage("/keybind");
        }
        if (addon != null)
        {
            var radioButton = (AtkComponentRadioButton*)addon->GetComponentByNodeId(3);
            if (!radioButton->IsSelected)
            {
                if (EzThrottler.Throttle("Open"))
                    Chat.Instance.SendMessage("/keybind");
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieves the keybind for the "Walk Forward" action from the keybind configuration menu.
    /// </summary>
    /// <returns>The keybind value as a byte, or 0 if not found.</returns>
    internal unsafe static byte GetWalkForwardKeyBind()
    {
        byte key = 0;
        TryGetAddonByName<AtkUnitBase>("ConfigKeybind", out var addon);
        if (addon != null)
        {
            var listNode = addon->GetComponentListById(16)->GetItemRenderer(1);
            var firstButtonNode = listNode->UldManager.RootNode->PrevSiblingNode->PrevSiblingNode->PrevSiblingNode;
            var secondButtonNode = firstButtonNode->PrevSiblingNode;
            var firstText = firstButtonNode->GetAsAtkComponentButton()->GetTextNodeById(5)->GetAsAtkTextNode()->NodeText;
            var secondText = secondButtonNode->GetAsAtkComponentButton()->GetTextNodeById(5)->GetAsAtkTextNode()->NodeText;
            byte firstValue = *firstText.StringPtr;
            byte secondValue = *secondText.StringPtr;
            if (firstValue != 0)
            {
                key = firstValue;
                return key;

            }
            else if (secondValue != 0)
            {
                key = secondValue;
                return key;
            }
            else
            {
                key = 0;
                return key;
            }
        }
        return key;
    }

    /// <summary>
    /// Finds the index of a specific map in the decipher window.
    /// </summary>
    /// <param name="mapName">The name of the map to search for.</param>
    /// <returns>The index of the map if found, otherwise -1.</returns>
    public unsafe static int FindDecipherIndex(string mapName)
    {

        TryGetAddonByName<AtkUnitBase>("SelectIconString", out var addon);
        for (int i = 0; i < addon->GetComponentListById(3)->ListLength; i++)
        {
            var itemName = addon->GetComponentListById(3)->GetItemRenderer(i)->GetTextNodeById(2)->GetAsAtkTextNode()->NodeText;
            if (itemName.ToString().Contains(mapName))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Retrieves the text from the "_ToDoList" addon to determine if the player has opened a coffer in the "Treasure Hunt."
    /// </summary>
    /// <returns>The text from the "_ToDoList" addon, or "None" if not found.</returns>
    public unsafe static string GetToDoText()
    {
        var text = "None";
        if (TryGetAddonByName<AtkUnitBase>("_ToDoList", out var addon))
        {
            text = addon->GetComponentByNodeId(4)->GetTextNodeById(10)->GetAsAtkTextNode()->NodeText.ToString();
        }
        return text;
    }

    /// <summary>
    /// Retrieves the subtext from the "_ToDoList" addon.
    /// </summary>
    /// <returns>The subtext from the "_ToDoList" addon, or an empty string if not found.</returns>
    public unsafe static string GetToDoSubText()
    {
        string text = "";
        if (TryGetAddonByName<AtkUnitBase>("_ToDoList", out var addon))
        {
            if (addon->GetRootNode()->ChildNode != null)
            {
                var child = addon->GetRootNode()->ChildNode;
                while (child != null) // Check if child is not null first
                {
                    if (child->NodeId == 22001)
                    {
                        text = "found";
                        text = child->GetComponent()->GetTextNodeById(6)->GetAsAtkTextNode()->NodeText.ToString();
                        return text; // Exit immediately if the node is found
                    }
                    child = child->PrevSiblingNode;
                }
            }
        }
        return text;
    }

    /// <summary>
    /// The default starting task when loading the plugin.
    /// </summary>
    private static string currentTask = "idle";

    /// <summary>
    /// Updates the current task displayed in the plugin's main window under the "Start" tab.
    /// </summary>
    /// <param name="task">The new task to display.</param>
    public static void UpdateCurrentTask(string task) { currentTask = task; }
}
