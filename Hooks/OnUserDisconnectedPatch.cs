using AutoBrazier.Services;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Stunlock.Network;

namespace AutoBrazier.Hooks;

[HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserDisconnected))]
public static class OnUserDisconnectedPatch
{
	private static void Prefix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId, ConnectionStatusChangeReason connectionStatusReason, string extraData)
	{
		if (Core.PlayerService == null) Core.InitializeAfterLoaded();
		try
		{
			var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
			var serverClient = __instance._ApprovedUsersLookup[userIndex];
			var userData = __instance.EntityManager.GetComponentData<User>(serverClient.UserEntity);
			bool isNewVampire = userData.CharacterName.IsEmpty;

			if (!isNewVampire)
			{
				var playerName = userData.CharacterName.ToString();
				Core.PlayerService.UpdatePlayerCache(serverClient.UserEntity, playerName, playerName, true);

				Core.Log.LogInfo($"Player {playerName} disconnected");
			}

            AutoToggle.PlayerDisconnected(__instance, netConnectionId, connectionStatusReason, extraData);
		}
		catch { };
	}
}