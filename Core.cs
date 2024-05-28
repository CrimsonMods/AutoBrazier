using System.Runtime.CompilerServices;
using BepInEx.Logging;
using ProjectM;
using ProjectM.Scripting;
using Unity.Entities;
using AutoBrazier.Server;
using Bloody.Core.API.v1;

namespace AutoBrazier;

internal static class Core
{
    public static World Server { get; } = GetWorld("Server") ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");

    public static EntityManager EntityManager { get; } = Server.EntityManager;
    public static GameDataSystem GameDataSystem { get; } = Server.GetExistingSystemManaged<GameDataSystem>();
    public static PrefabCollectionSystem PrefabCollectionSystem { get; internal set; }
    public static ServerScriptMapper ServerScriptMapper { get; internal set; }
    public static double ServerTime => ServerGameManager.ServerTime;
    public static ServerGameManager ServerGameManager => ServerScriptMapper.GetServerGameManager();

    public static ServerGameSettingsSystem ServerGameSettingsSystem { get; internal set; }

    public static ManualLogSource Log { get; } = Plugin._logger;

    public const int MAX_REPLY_LENGTH = 509;

    public static void LogException(System.Exception e, [CallerMemberName] string caller = null)
    {
        Log.LogError($"Failure in {caller}\nMessage: {e.Message} Inner:{e.InnerException?.Message}\n\nStack: {e.StackTrace}\nInner Stack: {e.InnerException?.StackTrace}");
    }

    internal static void InitializeAfterLoaded()
    {
        if (_hasInitialized) return;

        PrefabCollectionSystem = Server.GetExistingSystemManaged<PrefabCollectionSystem>();
        ServerGameSettingsSystem = Server.GetExistingSystemManaged<ServerGameSettingsSystem>();
        ServerScriptMapper = Server.GetExistingSystemManaged<ServerScriptMapper>();

        EventsHandlerSystem.OnUserConnected += AutoToggle.PlayerConnected;
        EventsHandlerSystem.OnUserDisconnected += AutoToggle.PlayerDisconnected;

        _hasInitialized = true;
        Log.LogInfo($"{nameof(InitializeAfterLoaded)} completed");
    }
    
    private static bool _hasInitialized = false;
    public static bool HasInitialized { get { return _hasInitialized; } }

    private static World GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name)
            {
                return world;
            }
        }

        return null;
    }
}