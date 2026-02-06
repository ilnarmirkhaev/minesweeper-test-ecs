using System.Collections.Generic;
using Core.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Systems
{
    public class FieldInitializationSystem : IEcsInitSystem
    {
        private readonly EcsPool<CellComponent> _cellPool;
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<NeighborMinesCount> _neighborCountPool;

        private static readonly Vector2Int[] NeighborOffsets =
        {
            new(-1, -1), new(0, -1), new(1, -1),
            new(-1,  0),             new(1,  0),
            new(-1,  1), new(0,  1), new(1,  1)
        };

        public FieldInitializationSystem(EcsPool<CellComponent> cellPool, EcsPool<MineComponent> minePool,
            EcsPool<NeighborMinesCount> neighborCountPool)
        {
            _cellPool = cellPool;
            _minePool = minePool;
            _neighborCountPool = neighborCountPool;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var allCellsFilter = world.Filter<CellComponent>().End();

            var positionToEntity = new Dictionary<Vector2Int, int>();
            foreach (var entity in allCellsFilter)
            {
                ref var cell = ref _cellPool.Get(entity);
                positionToEntity[cell.Position] = entity;
            }

            foreach (var entity in allCellsFilter)
            {
                ref var cell = ref _cellPool.Get(entity);
                var count = CountNeighborMines(cell.Position, positionToEntity);
                ref var neighborCount = ref _neighborCountPool.Add(entity);
                neighborCount.Value = count;
            }
        }

        private int CountNeighborMines(Vector2Int position, IReadOnlyDictionary<Vector2Int, int> positionToEntity)
        {
            var count = 0;
            foreach (var offset in NeighborOffsets)
            {
                var neighborPos = position + offset;
                if (!positionToEntity.TryGetValue(neighborPos, out var neighborEntity))
                    continue;
                if (_minePool.Has(neighborEntity))
                    count++;
            }

            return count;
        }
    }
}