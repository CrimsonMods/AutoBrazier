using Bloodstone.API;
using ProjectM.Network;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace AutoBrazier.Server
{
    internal class ManualToggle
    {
        private static readonly PrefabGUID BoneGuid = new(1821405450);

        private static void Toggle(Entity brazier, out bool isBurning)
        {
            var brazierComponentData = VWorld.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
            brazierComponentData.Enabled = !brazierComponentData.Enabled;
            VWorld.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
            isBurning = brazierComponentData.Enabled;
        }

        public static void SetBurning(Entity brazier, bool isBurning)
        {
            var brazierComponentData = VWorld.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
            brazierComponentData.Enabled = isBurning;
            VWorld.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
        }

        public static void SetBurning(NativeArray<Entity> braziers, bool isBurning)
        {
            foreach (var brazier in braziers)
            {
                var brazierComponentData = VWorld.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
                brazierComponentData.Enabled = isBurning;
                VWorld.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
            }
        }

        private static Vector3 ToVector3(LocalToWorld localToWorld)
        {
            return new Vector3(localToWorld.Position.x, localToWorld.Position.y, localToWorld.Position.z);
        }

        public static void AddBonesIfEmpty(FromCharacter fromCharacter, Entity brazier)
        {
            InventoryUtilities.TryGetInventoryEntity(VWorld.Server.EntityManager, fromCharacter.Character, out Entity playerInventory);
            InventoryUtilities.TryGetInventoryEntity(VWorld.Server.EntityManager, brazier, out Entity brazierInventory);
            var gameDataSystem = VWorld.Server.GetExistingSystemManaged<GameDataSystem>();

            int playerBoneCount = InventoryUtilities.GetItemAmount(VWorld.Server.EntityManager, playerInventory, BoneGuid);
            int brazierBoneCount = InventoryUtilities.GetItemAmount(VWorld.Server.EntityManager, brazierInventory, BoneGuid);
            if (playerBoneCount > 0 && brazierBoneCount == 0)
            {
                InventoryUtilities.TryGetItemSlot(VWorld.Server.EntityManager, playerInventory, BoneGuid, out int slotId);
                InventoryUtilitiesServer.SplitItemStacks(VWorld.Server.EntityManager, gameDataSystem.ItemHashLookupMap,
                    playerInventory, slotId);
                InventoryUtilitiesServer.TryMoveItem(VWorld.Server.EntityManager, gameDataSystem.ItemHashLookupMap,
                    playerInventory, slotId, brazierInventory);
            }
        }
    }
}
