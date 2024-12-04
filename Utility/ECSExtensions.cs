using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using ProjectM;
using Unity.Entities;
using Stunlock.Core;
using Unity.Collections;


namespace AutoBrazier.Utility
{
    //#pragma warning disable CS8500
    public static class ECSExtensions
    {
        public unsafe static void Write<T>(this Entity entity, T componentData) where T : struct
        {
            // Get the ComponentType for T
            var ct = new ComponentType(Il2CppType.Of<T>());

            // Marshal the component data to a byte array
            byte[] byteArray = StructureToByteArray(componentData);

            // Get the size of T
            int size = Marshal.SizeOf<T>();

            // Create a pointer to the byte array
            fixed (byte* p = byteArray)
            {
                // Set the component data
                Core.EntityManager.SetComponentDataRaw(entity, ct.TypeIndex, p, size);
            }
        }

        // Helper function to marshal a struct to a byte array
        public static byte[] StructureToByteArray<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf(structure);
            byte[] byteArray = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, byteArray, 0, size);
            Marshal.FreeHGlobal(ptr);

            return byteArray;
        }

        public unsafe static T Read<T>(this Entity entity) where T : struct
        {
            // Get the ComponentType for T
            var ct = new ComponentType(Il2CppType.Of<T>());

            // Get a pointer to the raw component data
            void* rawPointer = Core.EntityManager.GetComponentDataRawRO(entity, ct.TypeIndex);

            // Marshal the raw data to a T struct
            T componentData = Marshal.PtrToStructure<T>(new IntPtr(rawPointer));

            return componentData;
        }

        public static bool Has<T>(this Entity entity)
        {
            var ct = new ComponentType(Il2CppType.Of<T>());
            return Core.EntityManager.HasComponent(entity, ct);
        }

        public static string LookupName(this PrefabGUID prefabGuid)
        {
            var prefabCollectionSystem = Core.Server.GetExistingSystemManaged<PrefabCollectionSystem>();
            return (prefabCollectionSystem.PrefabGuidToNameDictionary.ContainsKey(prefabGuid)
                ? prefabCollectionSystem.PrefabGuidToNameDictionary[prefabGuid] + " " + prefabGuid : "GUID Not Found").ToString();
        }

        public static void Add<T>(this Entity entity)
        {
            var ct = new ComponentType(Il2CppType.Of<T>());
            Core.EntityManager.AddComponent(entity, ct);
        }

        public static void Remove<T>(this Entity entity)
        {
            var ct = new ComponentType(Il2CppType.Of<T>());
            Core.EntityManager.RemoveComponent(entity, ct);
        }

        public static NativeArray<Entity> GetEntitiesByComponentType<T1>(bool includeAll = false, bool includeDisabled = false, bool includeSpawn = false, bool includePrefab = false, bool includeDestroyed = false)
        {
            EntityQueryOptions options = EntityQueryOptions.Default;
            if (includeAll) options |= EntityQueryOptions.IncludeAll;
            if (includeDisabled) options |= EntityQueryOptions.IncludeDisabled;
            if (includeSpawn) options |= EntityQueryOptions.IncludeSpawnTag;
            if (includePrefab) options |= EntityQueryOptions.IncludePrefab;
            if (includeDestroyed) options |= EntityQueryOptions.IncludeDestroyTag;

            EntityQueryDesc queryDesc = new()
            {
                All = new ComponentType[] { new(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite) },
                Options = options
            };

            var query = Core.EntityManager.CreateEntityQuery(queryDesc);

            var entities = query.ToEntityArray(Allocator.Temp);
            return entities;
        }

        public static NativeArray<Entity> GetEntitiesByComponentTypes<T1, T2>(bool includeAll = false, bool includeDisabled = false, bool includeSpawn = false, bool includePrefab = false, bool includeDestroyed = false)
        {
            EntityQueryOptions options = EntityQueryOptions.Default;
            if (includeAll) options |= EntityQueryOptions.IncludeAll;
            if (includeDisabled) options |= EntityQueryOptions.IncludeDisabled;
            if (includeSpawn) options |= EntityQueryOptions.IncludeSpawnTag;
            if (includePrefab) options |= EntityQueryOptions.IncludePrefab;
            if (includeDestroyed) options |= EntityQueryOptions.IncludeDestroyTag;

            EntityQueryDesc queryDesc = new()
            {
                All = new ComponentType[] { new(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite), new(Il2CppType.Of<T2>(), ComponentType.AccessMode.ReadWrite) },
                Options = options
            };

            var query = Core.EntityManager.CreateEntityQuery(queryDesc);

            var entities = query.ToEntityArray(Allocator.Temp);
            return entities;
        }
    }
    //#pragma warning restore CS8500
}
