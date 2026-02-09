using Configs;
using Core.Components;
using Core.Services;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Systems
{
    public sealed class WinCheckSystem : IEcsRunSystem
    {
        private readonly MineFieldConfig _config;
        private readonly EcsPool<GameOverEvent> _gameOverPool;
        private readonly GameSessionState _session;

        private EcsFilter _openedFilter;
        private EcsFilter _explodedFilter;

        public WinCheckSystem(MineFieldConfig config, EcsPool<GameOverEvent> gameOverPool, GameSessionState session)
        {
            _config = config;
            _gameOverPool = gameOverPool;
            _session = session;
        }

        public void Run(IEcsSystems systems)
        {
            if (_session.IsGameOver || !_session.GameStarted) return;

            var world = systems.GetWorld();

            _openedFilter ??= world.Filter<CellComponent>().Inc<Opened>().End();
            _explodedFilter ??= world.Filter<CellComponent>().Inc<Exploded>().End();

            var isExploded = _explodedFilter.GetEntitiesCount() > 0;
            if (isExploded)
            {
                GameOver(false, world);
                return;
            }

            var openedSafe = _openedFilter.GetEntitiesCount();
            var totalSafe = _config.TotalCells - _config.MinesCount;
            totalSafe = Mathf.Max(1, totalSafe);

            if (openedSafe >= totalSafe)
            {
                GameOver(true, world);
            }
        }

        private void GameOver(bool win, EcsWorld world)
        {
            _session.IsGameOver = true;
            _session.IsWin = win;

            var evtEntity = world.NewEntity();
            ref var evt = ref _gameOverPool.Add(evtEntity);
            evt.IsWin = win;

            var result = win ? "Win" : "Lose";
            Debug.Log($"Game Over: {result}");
        }
    }
}