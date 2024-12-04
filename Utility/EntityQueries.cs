using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using Il2CppInterop.Runtime;

namespace AutoBrazier.Utility
{
    internal class EntityQueries
    {
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
