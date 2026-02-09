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
        private readonly GameSessionState _session;
        private readonly EcsWorld _world;

        private EcsFilter _openedFilter;
        private EcsFilter _explodedFilter;
        private EcsFilter _listenersFilter;

        public WinCheckSystem(MineFieldConfig config, GameSessionState session, EcsWorld world)
        {
            _config = config;
            _session = session;
            _world = world;
        }

        public void Run(IEcsSystems systems)
        {
            if (_session.IsGameOver || !_session.GameStarted) return;

            _openedFilter ??= _world.Filter<CellComponent>().Inc<Opened>().End();
            _explodedFilter ??= _world.Filter<CellComponent>().Inc<Exploded>().End();

            var isExploded = _explodedFilter.GetEntitiesCount() > 0;
            if (isExploded)
            {
                Lose();
                return;
            }

            var openedSafe = _openedFilter.GetEntitiesCount();
            var totalSafe = _config.TotalCells - _config.MinesCount;
            totalSafe = Mathf.Max(1, totalSafe);

            if (openedSafe >= totalSafe)
            {
                Win();
            }
        }

        private void Win() => GameOver(true);
        private void Lose() => GameOver(false);

        private void GameOver(bool win)
        {
            _session.IsGameOver = true;
            _session.IsWin = win;

            var pool = _world.GetPool<GameOverListenerBind>();
            _listenersFilter ??= _world.Filter<GameOverListenerBind>().End();
            foreach (var e in _listenersFilter)
            {
                pool.Get(e).Listener.GameOver(win);
            }

            var result = win ? "Win" : "Lose";
            Debug.Log($"Game Over: {result}");
        }
    }
}