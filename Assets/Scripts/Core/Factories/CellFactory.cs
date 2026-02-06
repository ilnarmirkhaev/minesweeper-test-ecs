using Core.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Factories
{
    public class CellFactory : IFactory<Vector2Int, CellComponent>
    {
        private readonly EcsWorld _world;

        public CellFactory(EcsWorld world)
        {
            _world = world;
        }

        public CellComponent Create(Vector2Int arg)
        {
            var entity = _world.NewEntity();
            var pool = _world.GetPool<CellComponent>();
            
            ref var cell = ref pool.Add(entity);
            return cell;
        }
    }
}