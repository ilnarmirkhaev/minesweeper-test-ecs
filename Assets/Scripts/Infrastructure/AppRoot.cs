using Configs;
using Core.Components;
using Core.Systems;
using Leopotam.EcsLite;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure
{
    public class AppRoot : LifetimeScope
    {
        [SerializeField] private MineFieldConfig _config;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_config);

            builder.RegisterEntryPoint<WorldRoot>();
            builder.RegisterInstance(new EcsWorld());

            RegisterSystems(builder);
            RegisterPools(builder);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterSystem<FieldCreationSystem>();
            builder.RegisterSystem<MineDistributionSystem>();
            builder.RegisterSystem<FieldInitializationSystem>();
            builder.RegisterSystem<DebugFieldLogSystem>();
        }

        private void RegisterPools(IContainerBuilder builder)
        {
            builder.RegisterPool<CellComponent>();
            builder.RegisterPool<MineComponent>();
            builder.RegisterPool<NeighborMinesCount>();
        }
    }
}