using HarmonyLib;
using ProjectM;

namespace AutoBrazier.Hooks
{
    [HarmonyPatch(typeof(SpawnTeamSystem_OnPersistenceLoad), nameof(SpawnTeamSystem_OnPersistenceLoad.OnUpdate))]
    internal class OnLoadPatches
    {
        [HarmonyPostfix]
        public static void OneShot_AfterLoad_InitializationPatch()
        {
            Core.InitializeAfterLoaded();
            Plugin._harmony.Unpatch(typeof(SpawnTeamSystem_OnPersistenceLoad).GetMethod("OnUpdate"), typeof(OnLoadPatches).GetMethod("OneShot_AfterLoad_InitializationPatch"));
        }
    }
}
