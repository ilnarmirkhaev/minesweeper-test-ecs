using System.Collections.Generic;
using Core.Components;
using Core.Services;
using Leopotam.EcsLite;
using Tools;
using UnityEngine;

namespace Core.Systems
{
    public sealed class CellOpenSystem : IEcsRunSystem
    {
        private readonly ICellLookup _lookup;

        private readonly EcsPool<CellComponent> _cellPool;
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<NeighborMinesCount> _neighborPool;
        private readonly EcsPool<Opened> _openedPool;
        private readonly EcsPool<Exploded> _explodedPool;
        private readonly EcsPool<Dirty> _dirtyPool;
        private readonly EcsPool<OpenCellCommand> _requestPool;
        private readonly EcsPool<Flagged> _flagPool;

        private readonly Queue<int> _queue = new();

        public CellOpenSystem(ICellLookup lookup, EcsPool<CellComponent> cellPool, EcsPool<MineComponent> minePool,
            EcsPool<NeighborMinesCount> neighborPool, EcsPool<Opened> openedPool, EcsPool<Exploded> explodedPool,
            EcsPool<Dirty> dirtyPool, EcsPool<OpenCellCommand> requestPool, EcsPool<Flagged> flagPool)
        {
            _lookup = lookup;
            _cellPool = cellPool;
            _minePool = minePool;
            _neighborPool = neighborPool;
            _openedPool = openedPool;
            _explodedPool = explodedPool;
            _dirtyPool = dirtyPool;
            _requestPool = requestPool;
            _flagPool = flagPool;
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<OpenCellCommand>().End();

            foreach (var e in filter)
            {
                ref var command = ref _requestPool.Get(e);
                OpenCell(command.CellEntity);
                world.DelEntity(e);
            }
        }

        private void OpenCell(int cellEntity)
        {
            if (_minePool.Has(cellEntity))
            {
                MarkCellExploded(cellEntity);
                return;
            }

            FloodFillOpen(cellEntity);
        }

        private void FloodFillOpen(int cellEntity)
        {
            _queue.Enqueue(cellEntity);

            while (_queue.Count > 0)
            {
                var e = _queue.Dequeue();
                if (_openedPool.Has(e) || _minePool.Has(e)) continue;

                MarkCellOpened(e);

                var value = _neighborPool.Has(e) ? _neighborPool.Get(e).Value : 0;
                if (value != 0) continue;

                ref var cell = ref _cellPool.Get(e);
                AddNeighborCellsToFlood(cell.Position, _queue);
            }
        }

        private void AddNeighborCellsToFlood(Vector2Int cellPosition, Queue<int> queue)
        {
            foreach (var offset in Constants.NeighborOffsets)
            {
                var pos = cellPosition + offset;
                if (!_lookup.TryGetCellEntity(pos, out var neighborEntity))
                    continue;
                if (_openedPool.Has(neighborEntity) || _minePool.Has(neighborEntity)) continue;

                queue.Enqueue(neighborEntity);
            }
        }

        private void MarkCellOpened(int entity)
        {
            if (_openedPool.Has(entity)) return;
            _openedPool.Add(entity);

            _flagPool.Del(entity);

            MarkDirty(entity);
        }

        private void MarkCellExploded(int entity)
        {
            _explodedPool.Add(entity);
            MarkDirty(entity);
        }

        private void MarkDirty(int entity)
        {
            if (_dirtyPool.Has(entity)) return;
            _dirtyPool.Add(entity);
        }
    }
}