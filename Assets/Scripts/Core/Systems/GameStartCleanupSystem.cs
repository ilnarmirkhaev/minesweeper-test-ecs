using Core.Components;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public sealed class GameStartCleanupSystem : IEcsRunSystem
    {
        private EcsFilter _gameStartFilter;

        public void Run(IEcsSystems systems)
        {
            _gameStartFilter ??= systems.GetWorld().Filter<FirstCellOpenedEvent>().End();

            foreach (var entity in _gameStartFilter)
            {
                systems.GetWorld().DelEntity(entity);
            }
        }
    }
}