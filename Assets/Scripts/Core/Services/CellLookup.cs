using System.Collections.Generic;
using Core.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Services
{
    public sealed class CellLookup : ICellLookup
    {
        private readonly EcsWorld _world;
        private readonly EcsPool<CellComponent> _cellPool;

        private Dictionary<Vector2Int, int> _map;

        public CellLookup(EcsWorld world, EcsPool<CellComponent> cellPool)
        {
            _world = world;
            _cellPool = cellPool;
        }

        public bool TryGetCellEntity(Vector2Int position, out int entity)
        {
            EnsureBuilt();
            return _map.TryGetValue(position, out entity);
        }

        private void EnsureBuilt()
        {
            if (_map != null)
                return;

            _map = new Dictionary<Vector2Int, int>();
            var filter = _world.Filter<CellComponent>().End();
            foreach (var entity in filter)
            {
                ref var cell = ref _cellPool.Get(entity);
                _map[cell.Position] = entity;
            }
        }
    }
}

