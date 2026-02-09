using Configs;
using Core.Components;
using Core.Services;
using Core.Systems;
using Leopotam.EcsLite;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UI;

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

            RegisterServices(builder);
            RegisterSystems(builder);
            RegisterPools(builder);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<GameSessionState>(Lifetime.Singleton);
            builder.Register<ICellLookup, CellLookup>(Lifetime.Singleton);
            builder.Register<ICellInputService, CellInputService>(Lifetime.Singleton);
            builder.Register<ICellViewRegistry, CellViewRegistry>(Lifetime.Singleton);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterSystem<FieldCreationSystem>();
            builder.RegisterSystem<SessionResetSystem>();
            builder.RegisterSystem<CellOpenSystem>();
            builder.RegisterSystem<CellFlagSystem>();
            builder.RegisterSystem<MineDistributionSystem>();
            builder.RegisterSystem<NeighborMinesCountSystem>();
            builder.RegisterSystem<DebugFieldLogSystem>();
            builder.RegisterSystem<GameStartSystem>();
            builder.RegisterSystem<RestartInputSystem>();
            builder.RegisterSystem<RestartSystem>();
            builder.RegisterSystem<WinCheckSystem>();
            builder.RegisterSystem<CellViewDrawSystem>();
        }

        private void RegisterPools(IContainerBuilder builder)
        {
            builder.RegisterPool<CellComponent>();
            builder.RegisterPool<MineComponent>();
            builder.RegisterPool<NeighborMinesCount>();

            builder.RegisterPool<Opened>();
            builder.RegisterPool<Flagged>();
            builder.RegisterPool<Exploded>();
            builder.RegisterPool<Dirty>();

            builder.RegisterPool<FirstCellOpenedEvent>();
            builder.RegisterPool<OpenCellRequest>();
            builder.RegisterPool<ToggleFlagRequest>();
            builder.RegisterPool<RestartRequest>();
            builder.RegisterPool<GameOverEvent>();
        }
    }
}