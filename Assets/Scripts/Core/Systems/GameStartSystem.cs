using Core.Components;
using Core.Services;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public sealed class GameStartSystem : IEcsRunSystem
    {
        private readonly GameSessionState _session;

        private EcsFilter _gameStartFilter;

        public GameStartSystem(GameSessionState session)
        {
            _session = session;
        }

        public void Run(IEcsSystems systems)
        {
            _gameStartFilter ??= systems.GetWorld().Filter<FirstCellOpenedEvent>().End();

            foreach (var entity in _gameStartFilter)
            {
                // _session.Reset();
                systems.GetWorld().DelEntity(entity);
            }
        }
    }
}
