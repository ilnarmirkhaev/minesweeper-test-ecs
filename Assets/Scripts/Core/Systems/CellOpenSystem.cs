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
        private readonly EcsWorld _world;
        private readonly ICellLookup _lookup;
        private readonly GameSessionState _session;

        private readonly EcsPool<CellComponent> _cellPool;
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<NeighborMinesCount> _neighborPool;
        private readonly EcsPool<Opened> _openedPool;
        private readonly EcsPool<Flagged> _flaggedPool;
        private readonly EcsPool<Exploded> _explodedPool;
        private readonly EcsPool<Dirty> _dirtyPool;
        private readonly EcsPool<OpenCellRequest> _requestPool;
        private readonly EcsPool<FirstCellOpenedEvent> _firstCellPool;

        public CellOpenSystem(EcsWorld world, ICellLookup lookup, GameSessionState session,
            EcsPool<CellComponent> cellPool, EcsPool<MineComponent> minePool, EcsPool<NeighborMinesCount> neighborPool,
            EcsPool<Opened> openedPool, EcsPool<Flagged> flaggedPool, EcsPool<Exploded> explodedPool,
            EcsPool<Dirty> dirtyPool, EcsPool<OpenCellRequest> requestPool, EcsPool<FirstCellOpenedEvent> firstCellPool)
        {
            _world = world;
            _lookup = lookup;
            _session = session;
            _cellPool = cellPool;
            _minePool = minePool;
            _neighborPool = neighborPool;
            _openedPool = openedPool;
            _flaggedPool = flaggedPool;
            _explodedPool = explodedPool;
            _dirtyPool = dirtyPool;
            _requestPool = requestPool;
            _firstCellPool = firstCellPool;
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<OpenCellRequest>().End();

            foreach (var reqEntity in filter)
            {
                ref var req = ref _requestPool.Get(reqEntity);
                Handle(req.Position);
                world.DelEntity(reqEntity);
            }
        }

        private void Handle(Vector2Int position)
        {
            if (_session.IsGameOver)
                return;

            if (!_lookup.TryGetCellEntity(position, out var cellEntity))
                return;

            if (_flaggedPool.Has(cellEntity) || _openedPool.Has(cellEntity))
                return;

            OpenCell(cellEntity);

            if (!_session.GameStarted)
            {
                _session.GameStarted = true;
                _firstCellPool.Add(_world.NewEntity());
            }
        }

        private void OpenCell(int cellEntity)
        {
            if (!_session.GameStarted)
            {
                MarkCellOpened(cellEntity);
                return;
            }

            if (_minePool.Has(cellEntity))
            {
                _explodedPool.Add(cellEntity);
                MarkDirty(cellEntity);
                return;
            }

            FloodFillOpen(cellEntity);
        }

        private void FloodFillOpen(int cellEntity)
        {
            var queue = new Queue<int>();
            queue.Enqueue(cellEntity);

            while (queue.Count > 0)
            {
                var e = queue.Dequeue();
                if (_openedPool.Has(e) || _flaggedPool.Has(e))
                    continue;
                if (_minePool.Has(e))
                    continue;

                MarkCellOpened(e);

                var value = _neighborPool.Has(e) ? _neighborPool.Get(e).Value : 0;
                if (value != 0) continue;

                ref var cell = ref _cellPool.Get(e);
                AddNeighborCellsToFlood(cell.Position, queue);
            }
        }

        private void AddNeighborCellsToFlood(Vector2Int cellPosition, Queue<int> queue)
        {
            foreach (var offset in Constants.NeighborOffsets)
            {
                var pos = cellPosition + offset;
                if (!_lookup.TryGetCellEntity(pos, out var neighborEntity))
                    continue;
                if (_openedPool.Has(neighborEntity) || _flaggedPool.Has(neighborEntity))
                    continue;
                if (_minePool.Has(neighborEntity))
                    continue;

                queue.Enqueue(neighborEntity);
            }
        }

        private void MarkCellOpened(int entity)
        {
            if (_openedPool.Has(entity)) return;
            _openedPool.Add(entity);
            MarkDirty(entity);
        }

        private void MarkDirty(int entity)
        {
            if (!_dirtyPool.Has(entity))
                _dirtyPool.Add(entity);
        }
    }
}