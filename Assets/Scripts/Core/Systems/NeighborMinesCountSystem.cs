using Core.Components;
using Core.Services;
using Leopotam.EcsLite;
using Tools;
using UnityEngine;

namespace Core.Systems
{
    public class NeighborMinesCountSystem : IEcsRunSystem
    {
        private readonly EcsPool<CellComponent> _cellPool;
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<NeighborMinesCount> _neighborCountPool;
        private readonly ICellLookup _lookup;

        private EcsFilter _filter;
        private EcsFilter _safeCells;

        public NeighborMinesCountSystem(EcsPool<CellComponent> cellPool, EcsPool<MineComponent> minePool,
            EcsPool<NeighborMinesCount> neighborCountPool, ICellLookup lookup)
        {
            _cellPool = cellPool;
            _minePool = minePool;
            _neighborCountPool = neighborCountPool;
            _lookup = lookup;
        }

        public void Run(IEcsSystems systems)
        {
            _filter ??= systems.GetWorld().Filter<FirstCellClickedEvent>().End();

            if (_filter.GetEntitiesCount() <= 0) return;

            _safeCells ??= systems.GetWorld().Filter<CellComponent>().Exc<MineComponent>().End();

            foreach (var entity in _safeCells)
            {
                ref var cell = ref _cellPool.Get(entity);
                var count = CountNeighborMines(cell.Position);
                ref var neighborCount = ref _neighborCountPool.Add(entity);
                neighborCount.Value = count;
            }
        }

        private int CountNeighborMines(Vector2Int position)
        {
            var count = 0;
            foreach (var offset in Constants.NeighborOffsets)
            {
                var neighborPos = position + offset;
                if (!_lookup.TryGetCellEntity(neighborPos, out var neighborEntity)) continue;

                if (_minePool.Has(neighborEntity))
                {
                    count++;
                }
            }

            return count;
        }
    }
}