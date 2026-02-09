using System;
using System.Collections.Generic;
using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using VContainer.Unity;

namespace Infrastructure
{
    public class WorldRoot : IInitializable, ITickable, IDisposable
    {
        private readonly EcsWorld _world;
        private readonly IEnumerable<IEcsSystem> _systems;

        private EcsSystems _systemsGroup;

        public WorldRoot(EcsWorld world, IEnumerable<IEcsSystem> systems)
        {
            _world = world;
            _systems = systems;
        }

        public void Initialize() => InitializeSystems();

        private void InitializeSystems()
        {
            _systemsGroup = new EcsSystems(_world);
            _systemsGroup.ConvertScene();

            foreach (var system in _systems)
            {
                _systemsGroup.Add(system);
            }

#if UNITY_EDITOR
            _systemsGroup.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _systemsGroup.Add(new Leopotam.EcsLite.UnityEditor.EcsSystemsDebugSystem());
#endif

            _systemsGroup.Init();
        }

        public void Tick()
        {
            _systemsGroup?.Run();
        }

        public void Dispose()
        {
            _systemsGroup?.Destroy();
            _world?.Destroy();
        }
    }
}