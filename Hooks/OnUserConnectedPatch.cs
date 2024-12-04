using System;
using AutoBrazier.Services;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Stunlock.Network;

namespace AutoBrazier.Hooks;

[HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserConnected))]
public static class OnUserConnectedPatch
{
    public static void Postfix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId)
    {
        if(Core.PlayerService == null) Core.InitializeAfterLoaded();
        try
        {
            var entityManager = __instance.EntityManager;
            var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
            var  serverClient = __instance._ApprovedUsersLookup[userIndex];
            var userEntity = serverClient.UserEntity;
            var userData = entityManager.GetComponentData<User>(userEntity);

            if(!userData.CharacterName.IsEmpty)
            {
                var playerName = userData.CharacterName.ToString();
                Core.PlayerService.UpdatePlayerCache(userEntity, playerName, playerName);
            }

            AutoToggle.PlayerConnected(__instance, netConnectionId);
        }
        catch(Exception ex)
        {
            Plugin.Log(ex.Message);
        }
    }
}