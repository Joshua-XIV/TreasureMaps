using ECommons.Automation;
namespace TreasureMaps.Helpers;

// TODO: Use IPCs instead of in-game commands
public static class PluginManager
{
    /// <summary>
    /// Determines which auto-rotation plugin is installed.
    /// </summary>
    /// <returns>
    /// 1 if RotationSolver is installed,
    /// 2 if Wrath is installed,
    /// 3 if BossMod is installed,
    /// 0 if no supported auto-rotation plugin is found.
    /// </returns>
    public static byte GetAutoRotationPlugin()
    {
        if (Generic.IsPluginInstalled("RotationSolver"))
        {
            return 1;
        }
        else if (Generic.IsPluginInstalled("Wrath"))
        {
            return 2;
        }
        else if (Generic.IsPluginInstalled("BossMod"))
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Enables the auto-rotation feature of the installed plugin (if supported).
    /// </summary>
    public static void EnableAutoRotationPlugin()
    {
        var plugin = GetAutoRotationPlugin();
        if (plugin == 1)
        {
            Chat.Instance.SendMessage("/rsr manual");
        }
        else
        {
            DuoLog.Information("No Auto Rotation plugin found.");
        }
    }

    /// <summary>
    /// Disables the auto-rotation feature of the installed plugin (if supported).
    /// </summary>
    public static void DisableAutoRotationPlugin()
    {
        var plugin = GetAutoRotationPlugin();
        if (plugin == 1)
        {
            Chat.Instance.SendMessage("/rsr off");
        }
    }

    /// <summary>
    /// Enables the AI feature of BossMod.
    /// </summary>
    public static void EnableBossMod()
    {
        if (Generic.IsPluginInstalled("BossModReborn") && C.bossModRebornPlugin)
        {
            Chat.Instance.SendMessage("/bmrai on");
            Chat.Instance.SendMessage($"/bmrai maxdistancetarget {Distance.GetRange()}");
        }
        else if (Generic.IsPluginInstalled("BossMod") && C.bossModRebornPlugin)
        {
            Chat.Instance.SendMessage("/vbmai on");
        }
    }

    /// <summary>
    /// Disables the AI feature of BossMod.
    /// </summary>
    public static void DisableBossMod()
    {
        if (Generic.IsPluginInstalled("BossModReborn") && C.bossModRebornPlugin)
        {
            Chat.Instance.SendMessage("/bmrai off");
        }
        else if (Generic.IsPluginInstalled("BossMod") && C.bossModRebornPlugin)
        {
            Chat.Instance.SendMessage("/vbmai off");
        }
    }
}
