using Bloodstone.API;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using Il2CppInterop.Runtime;

namespace AutoBrazier.Server
{
    internal class EntityQueries
    {
        public static World Server { get; } = GetWorld("Server") ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");

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

        public static EntityManager EntityManager { get; } = Server.EntityManager;

        private static readonly ComponentType[] BrazierComponents =
        {
            ComponentType.ReadOnly(Il2CppType.Of<Bonfire>())
        };

        public static NativeArray<Entity> GetUserEntities()
        {
            var userQuery = Core.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>());
            return userQuery.ToEntityArray(Allocator.Temp);
        }

        public static NativeArray<Entity> GetBonfireEntities()
        {
            var query = Core.EntityManager.CreateEntityQuery(BrazierComponents);
            return query.ToEntityArray(Allocator.Temp);
        }
    }
}
