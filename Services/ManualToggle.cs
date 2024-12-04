using ProjectM.Network;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace AutoBrazier.Services
{
    internal class ManualToggle
    {
        private static readonly PrefabGUID BoneGuid = new(1821405450);

        private static void Toggle(Entity brazier, out bool isBurning)
        {
            var brazierComponentData = Core.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
            brazierComponentData.Enabled = !brazierComponentData.Enabled;
            Core.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
            isBurning = brazierComponentData.Enabled;
        }

        public static void SetBurning(Entity brazier, bool isBurning)
        {
            var brazierComponentData = Core.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
            brazierComponentData.Enabled = isBurning;
            Core.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
        }

        public static void SetBurning(NativeArray<Entity> braziers, bool isBurning)
        {
            foreach (var brazier in braziers)
            {
                var brazierComponentData = Core.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
                brazierComponentData.Enabled = isBurning;
                Core.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
            }
        }

        private static Vector3 ToVector3(LocalToWorld localToWorld)
        {
            return new Vector3(localToWorld.Position.x, localToWorld.Position.y, localToWorld.Position.z);
        }

        public static void AddBonesIfEmpty(FromCharacter fromCharacter, Entity brazier)
        {
            InventoryUtilities.TryGetInventoryEntity(Core.Server.EntityManager, fromCharacter.Character, out Entity playerInventory);
            InventoryUtilities.TryGetInventoryEntity(Core.Server.EntityManager, brazier, out Entity brazierInventory);
            var gameDataSystem = Core.Server.GetExistingSystemManaged<GameDataSystem>();

            int playerBoneCount = InventoryUtilities.GetItemAmount(Core.Server.EntityManager, playerInventory, BoneGuid);
            int brazierBoneCount = InventoryUtilities.GetItemAmount(Core.Server.EntityManager, brazierInventory, BoneGuid);
            if (playerBoneCount > 0 && brazierBoneCount == 0)
            {
                InventoryUtilities.TryGetItemSlot(Core.Server.EntityManager, playerInventory, BoneGuid, out int slotId);
                InventoryUtilitiesServer.SplitItemStacks(Core.Server.EntityManager, gameDataSystem.ItemHashLookupMap,
                    playerInventory, slotId);
                InventoryUtilitiesServer.TryMoveItem(Core.Server.EntityManager, gameDataSystem.ItemHashLookupMap,
                    playerInventory, slotId, brazierInventory);
            }
        }
    }
}
