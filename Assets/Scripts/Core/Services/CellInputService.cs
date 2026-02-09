using Core.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Services
{
    public sealed class CellInputService : ICellInputService
    {
        private readonly EcsWorld _world;
        private readonly EcsPool<OpenCellRequest> _openPool;
        private readonly EcsPool<ToggleFlagRequest> _flagPool;
        private readonly EcsPool<RestartRequest> _restartPool;
        private readonly GameSessionState _session;

        public CellInputService(EcsWorld world, EcsPool<OpenCellRequest> openPool, EcsPool<ToggleFlagRequest> flagPool,
            EcsPool<RestartRequest> restartPool, GameSessionState session)
        {
            _world = world;
            _openPool = openPool;
            _flagPool = flagPool;
            _restartPool = restartPool;
            _session = session;
        }

        public void ClickCell(Vector2Int position, CellClickButton button)
        {
            if (_session.IsGameOver) return;

            var cell = _world.NewEntity();
            if (button == CellClickButton.Left)
            {
                ref var req = ref _openPool.Add(cell);
                req.Position = position;
            }
            else
            {
                ref var req = ref _flagPool.Add(cell);
                req.Position = position;
            }
        }

        public void RequestRestart()
        {
            var e = _world.NewEntity();
            _restartPool.Add(e);
        }
    }
}

