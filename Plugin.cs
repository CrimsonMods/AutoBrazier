using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;

namespace AutoBrazier;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public static Harmony _harmony;
    public static ConfigEntry<bool> AutoToggleEnabled;

    public static ManualLogSource _logger;

    private void InitConfig()
    {
        AutoToggleEnabled = Config.Bind("Server", "autoToggleEnabled", true,
            "Turn braziers on when day starts, and off during the night starts, for online players/clans only.");
    }

    public override void Load()
    {
        _logger = base.Log;

        InitConfig();

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        Log($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");
    }

    public override bool Unload()
    {
        Config.Clear();
        _harmony?.UnpatchSelf();
        return true;
    }

    public enum LogSystem
    {
        Core
    }

    public new static void Log(string message, LogSystem system = LogSystem.Core, LogLevel logLevel = LogLevel.Info, bool forceLog = false)
    {
        _logger.Log(logLevel, ToLogMessage(system, message));
    }

    private static string ToLogMessage(LogSystem logSystem, string message)
    {
        return $"{DateTime.Now:u}: [{Enum.GetName(logSystem)}] {message}";
    }
}
