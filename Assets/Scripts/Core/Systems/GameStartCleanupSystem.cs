using Core.Components;
using Core.Services;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public sealed class GameStartCleanupSystem : IEcsRunSystem
    {
        private readonly GameSessionState _session;

        private EcsFilter _gameStartFilter;

        public GameStartCleanupSystem(GameSessionState session)
        {
            _session = session;
        }

        public void Run(IEcsSystems systems)
        {
            _gameStartFilter ??= systems.GetWorld().Filter<FirstCellClickedEvent>().End();

            var any = false;
            foreach (var entity in _gameStartFilter)
            {
                systems.GetWorld().DelEntity(entity);
                any = true;
            }

            if (any)
            {
                _session.GameStarted = true;
            }
        }
    }
}