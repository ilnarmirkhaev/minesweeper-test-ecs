using Core.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Services
{
    public sealed class CellInputService : ICellInputService
    {
        private readonly EcsWorld _world;
        private readonly GameSessionState _session;
        private readonly EcsPool<OpenCellRequest> _openPool;
        private readonly EcsPool<ToggleFlagRequest> _flagPool;
        private readonly EcsPool<RestartRequest> _restartPool;
        private readonly EcsPool<FirstCellClickedEvent> _firstCellPool;

        public CellInputService(EcsWorld world, GameSessionState session, EcsPool<OpenCellRequest> openPool,
            EcsPool<ToggleFlagRequest> flagPool, EcsPool<RestartRequest> restartPool,
            EcsPool<FirstCellClickedEvent> firstCellPool)
        {
            _world = world;
            _session = session;
            _openPool = openPool;
            _flagPool = flagPool;
            _restartPool = restartPool;
            _firstCellPool = firstCellPool;
        }

        public void ClickCell(Vector2Int position, CellClickButton button)
        {
            if (_session.IsGameOver) return;

            var cell = _world.NewEntity();
            if (button == CellClickButton.Left)
            {
                CheckFirstCellOpened(position);
                
                ref var req = ref _openPool.Add(cell);
                req.Position = position;
            }
            else
            {
                ref var req = ref _flagPool.Add(cell);
                req.Position = position;
            }
        }

        private void CheckFirstCellOpened(Vector2Int position)
        {
            if (_session.GameStarted) return;
            var e = _world.NewEntity();
            ref var cell = ref _firstCellPool.Add(e);
            cell.Position = position;
        }

        public void RequestRestart()
        {
            var e = _world.NewEntity();
            _restartPool.Add(e);
        }
    }
}

