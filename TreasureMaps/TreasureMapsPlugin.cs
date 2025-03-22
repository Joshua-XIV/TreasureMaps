using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Automation.LegacyTaskManager;
using ECommons.Configuration;
using ECommons.DalamudServices;
using System.Linq.Expressions;
using TreasureMaps.IPC;
using TreasureMaps.Scheduler;
using TreasureMaps.Toasts;
using Dalamud.Game.Command;
using FFXIVClientStructs;
using Dalamud.Game;
using Dalamud.IoC;
using TreasureMaps.Helpers;
using TreasureMaps.UI.MainWindow;
using TreasureMaps.UI.MainWindow.SettingsTabUI;

namespace TreasureMaps;
public class Plugin : IDalamudPlugin
{
    public string Name => "Treasure Maps";
    internal static Plugin P = null!;
    private Config config;
    public static Config C => P.config;

    internal TreasureMapsFinder mapFinder { get; private set; }
    // Services
    internal IToastGui toastGui { get; private init; } = null!;
    public Toast toast { get; }

    [PluginService]
    internal IDataManager DataManager { get; init; } = null!;

    [PluginService]
    internal IGameGui GameGui { get; init; } = null!;

    [PluginService]
    internal ISigScanner SigScanner { get; init; } = null!;

    [PluginService]
    internal IGameInteropProvider GameInteropProvider { get; init; } = null!;

    // Internal Windows
    internal WindowSystem windowSystem;
    internal MainWindow mainWindow;
    internal SettingsTab settingsWindow;

    // TaskManager
    internal TaskManager taskManager;
    
    // IPCs
    internal NavmeshIPC navmesh;

    #pragma warning disable CS8618
    public Plugin(IDalamudPluginInterface pluginInterface, IToastGui toastGui)
    #pragma warning restore CS8618
    {
        P = this;
        this.toastGui = toastGui;
        this.toast = new Toast();
        ECommonsMain.Init(pluginInterface, P, ECommons.Module.DalamudReflector, ECommons.Module.ObjectFunctions);
        new ECommons.Schedulers.TickScheduler(Load);
    }

    public void Load()
    {
        EzConfig.Migrate<Config>();
        config = EzConfig.Init<Config>();

        // Windows
        windowSystem = new();
        mainWindow = new();
        settingsWindow = new();

        // Etc
        taskManager = new();
        navmesh = new();
        mapFinder = new TreasureMapsFinder(this);

        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;

        Svc.PluginInterface.UiBuilder.OpenMainUi += () =>
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
        };

        Svc.Framework.Update += Tick;
        EzCmd.Add("/tmaps", OnCommand,
        """ 
                Opens Plugin Interface
                /tmaps r|run -> Starts Plugin
                /tmaps stop -> Stops Bunnies
                """);
    }

    private void Tick(object _)
    {
        if (SchedulerMain.runPlugin && Svc.ClientState.LocalPlayer != null)
        {
            SchedulerMain.Tick();
        }
    }
    
    public void Dispose()
    {
        this.mapFinder.Dispose();
        this.toast.Dispose();
        ECommonsMain.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        if (args.EqualsIgnoreCaseAny("r", "ru", "run", "runs"))
        {
            if (!C.acceptedDisclaimer)
                DuoLog.Error("Cannot Run Command Until Disclaimer is Accepted!");
            else
                SchedulerMain.EnablePlugin();
        }

        else if (args.EqualsIgnoreCaseAny("s", "st", "sto", "stop", "stops"))
        {
            if (!C.acceptedDisclaimer)
                DuoLog.Error("Cannot Run Command Until Disclaimer is Accepted!");
            else
                SchedulerMain.DisablePlugin();
        }

        else
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
        }
    }
}