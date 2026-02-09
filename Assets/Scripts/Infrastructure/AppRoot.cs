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
            // input systems
            builder.RegisterSystem<RestartInputSystem>();
            builder.RegisterSystem<CellClickSystem>();

            // init systems
            builder.RegisterSystem<FieldCreationSystem>();
            builder.RegisterSystem<GameResetSystem>();
            builder.RegisterSystem<MineDistributionSystem>();
            builder.RegisterSystem<NeighborMinesCountSystem>();
#if UNITY_EDITOR
            builder.RegisterSystem<DebugFieldLogSystem>();
#endif
            builder.RegisterSystem<GameStartCleanupSystem>();

            // game loop systems
            builder.RegisterSystem<CellOpenSystem>();
            builder.RegisterSystem<CellFlagSystem>();
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

            builder.RegisterPool<FirstCellClickedEvent>();
            builder.RegisterPool<ClickCellRequest>();
            builder.RegisterPool<OpenCellCommand>();
            builder.RegisterPool<ToggleFlagRequest>();
            builder.RegisterPool<RestartRequest>();
            builder.RegisterPool<GameResetListenerBind>();
            builder.RegisterPool<GameOverListenerBind>();
        }
    }
}