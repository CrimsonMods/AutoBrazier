using Bloodstone.API;
using Bloody.Core.Models.v1;
using Bloody.Core.GameData.v1;
using ProjectM;
using ProjectM.Network;
using Stunlock.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace AutoBrazier.Server
{
    internal class AutoToggle
    {
        private static TimeOfDay currentTimeOfDay;
        public static void OnTimeOfDayChanged(TimeOfDay timeOfDay)
        {
            currentTimeOfDay = timeOfDay;
            switch (timeOfDay)
            {
                case TimeOfDay.Night:
                    TurnOffAllBonfires();
                    return;
                case TimeOfDay.Day:
                    TurnOnBonfiresForOnlinePlayersOnly();
                    break;
                case TimeOfDay.All:
                    // ???
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay, null);
            }
        }

        private static void TurnOffAllBonfires()
        {
            var bonfireEntities = EntityQueries.GetBonfireEntities();
            ManualToggle.SetBurning(bonfireEntities, false);
        }

        private static void TurnOnBonfiresForOnlinePlayersOnly()
        {
            if (currentTimeOfDay == TimeOfDay.Night) return;

            var userEntities = EntityQueries.GetUserEntities();
            var onlineTeams = new List<Team>();
            foreach (var userEntity in userEntities)
            {
                var user = VWorld.Server.EntityManager.GetComponentData<User>(userEntity);
                if (!user.IsConnected) continue;
                var userTeam = VWorld.Server.EntityManager.GetComponentData<Team>(userEntity);
                onlineTeams.Add(userTeam);
            }

            var onlineBraziers = GetBonfires(onlineTeams, true);
            var offlineBraziers = GetBonfires(onlineTeams, false);

            foreach (var bonefire in offlineBraziers)
            {
                ManualToggle.SetBurning(bonefire, false);
            }

            foreach (var bonfire in onlineBraziers)
            {
                ManualToggle.SetBurning(bonfire, true);
            }
        }

        public static void PlayerConnected(ServerBootstrapSystem bootstrapSystem, NetConnectionId player)
        {
            if (currentTimeOfDay != TimeOfDay.Day) return;

            var userIndex = bootstrapSystem._NetEndPointToApprovedUserIndex[player];
            var serverClient = bootstrapSystem._ApprovedUsersLookup[userIndex];
            var userEntity = serverClient.UserEntity;

            var bonfireTeam = EntityQueries.GetBonfireEntities();

            foreach (var bonfire in bonfireTeam)
            {
                if (Core.ServerGameManager.IsAllies(bonfire, userEntity))
                {
                    ManualToggle.SetBurning(bonfire, true);
                }
            }
        }

        public static void PlayerDisconnected(ServerBootstrapSystem bootstrapSystem, NetConnectionId player, ConnectionStatusChangeReason reason, string extraData)
        {
            if (currentTimeOfDay == TimeOfDay.Night) return;

            var userIndex = bootstrapSystem._NetEndPointToApprovedUserIndex[player];
            var serverClient = bootstrapSystem._ApprovedUsersLookup[userIndex];
            var userEntity = serverClient.UserEntity;

            List<UserModel> onlineUsers = GameData.Users.Online.ToList();

            foreach (var onlineUser in onlineUsers)
            {
                if (Core.ServerGameManager.IsAllies(onlineUser.Entity, userEntity))
                {
                    return;
                }
            }

            var bonfireTeam = EntityQueries.GetBonfireEntities();
            foreach (var bonfire in bonfireTeam)
            {
                if (Core.ServerGameManager.IsAllies(bonfire, userEntity))
                {
                    ManualToggle.SetBurning(bonfire, false);
                }
            }
        }

        private static List<Entity> GetBonfires(List<Team> onlineTeams, bool isOnlineCheck)
        {
            var entities = EntityQueries.GetBonfireEntities();
            var onlineBraziers = new List<Entity>();
            var offlineBraziers = new List<Entity>();
            foreach (var entity in entities)
            {
                var bonfireTeam = Core.EntityManager.GetComponentData<Team>(entity);
                if (onlineTeams.Any(onlineTeam => Core.ServerGameManager.IsAllies(bonfireTeam, onlineTeam)))
                {
                    onlineBraziers.Add(entity);
                }
                else
                { 
                    offlineBraziers.Add(entity);
                }
            }

            if (isOnlineCheck)
            {
                return onlineBraziers;
            }
            else
            {
                return offlineBraziers;
            }
        }
    }
}
