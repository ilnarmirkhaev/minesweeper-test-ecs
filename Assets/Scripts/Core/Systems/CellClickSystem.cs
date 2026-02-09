using Core.Components;
using Core.Services;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Systems
{
    public class CellClickSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world;
        private readonly ICellLookup _lookup;
        private readonly GameSessionState _session;

        private readonly EcsPool<Opened> _openedPool;
        private readonly EcsPool<Flagged> _flaggedPool;
        private readonly EcsPool<ClickCellRequest> _requestPool;
        private readonly EcsPool<OpenCellCommand> _openPool;
        private readonly EcsPool<FirstCellClickedEvent> _firstCellPool;

        private EcsFilter _requestFilter;

        public CellClickSystem(EcsWorld world, ICellLookup lookup, GameSessionState session, EcsPool<Opened> openedPool,
            EcsPool<Flagged> flaggedPool, EcsPool<ClickCellRequest> requestPool, EcsPool<OpenCellCommand> openPool,
            EcsPool<FirstCellClickedEvent> firstCellPool)
        {
            _world = world;
            _lookup = lookup;
            _session = session;
            _openedPool = openedPool;
            _flaggedPool = flaggedPool;
            _requestPool = requestPool;
            _openPool = openPool;
            _firstCellPool = firstCellPool;
        }

        public void Run(IEcsSystems systems)
        {
            _requestFilter ??= _world.Filter<ClickCellRequest>().End();

            foreach (var reqEntity in _requestFilter)
            {
                ref var req = ref _requestPool.Get(reqEntity);
                Handle(req.Position);
                _world.DelEntity(reqEntity);
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

            CheckFirstCellOpened(cellEntity);
            SendOpenCommand(cellEntity);
        }

        private void SendOpenCommand(int cellEntity)
        {
            var e = _world.NewEntity();
            ref var command = ref _openPool.Add(e);
            command.CellEntity = cellEntity;
        }

        private void CheckFirstCellOpened(int cellEntity)
        {
            if (_session.GameStarted) return;
            var e = _world.NewEntity();
            ref var cell = ref _firstCellPool.Add(e);
            cell.CellEntity = cellEntity;
        }
    }
}