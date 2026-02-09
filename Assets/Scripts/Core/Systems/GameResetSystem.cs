using Core.Components;
using Core.Services;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public sealed class GameResetSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly GameSessionState _session;
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<NeighborMinesCount> _neighborPool;
        private readonly EcsPool<Opened> _openedPool;
        private readonly EcsPool<Flagged> _flaggedPool;
        private readonly EcsPool<Exploded> _explodedPool;
        private readonly EcsPool<Dirty> _dirtyPool;
        private readonly EcsPool<GameResetListenerBind> _listeners;

        private EcsFilter _restartFilter;
        private EcsFilter _cellsFilter;
        private EcsFilter _resetFilter;

        public GameResetSystem(GameSessionState session, EcsPool<MineComponent> minePool,
            EcsPool<NeighborMinesCount> neighborPool, EcsPool<Opened> openedPool, EcsPool<Flagged> flaggedPool,
            EcsPool<Exploded> explodedPool, EcsPool<Dirty> dirtyPool, EcsPool<GameResetListenerBind> listeners)
        {
            _session = session;
            _minePool = minePool;
            _neighborPool = neighborPool;
            _openedPool = openedPool;
            _flaggedPool = flaggedPool;
            _explodedPool = explodedPool;
            _dirtyPool = dirtyPool;
            _listeners = listeners;
        }

        public void Init(IEcsSystems systems)
        {
            _session.Reset();
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _restartFilter ??= world.Filter<RestartRequest>().End();
            var restart = false;
            foreach (var reqEntity in _restartFilter)
            {
                restart = true;
                world.DelEntity(reqEntity);
            }

            if (!restart) return;

            _session.Reset();

            RemoveComponentsFromCells(world);
            NotifyAboutGameReset(world);
        }

        private void RemoveComponentsFromCells(EcsWorld world)
        {
            _cellsFilter ??= world.Filter<CellComponent>().End();
            foreach (var cellEntity in _cellsFilter)
            {
                _minePool.Del(cellEntity);
                _neighborPool.Del(cellEntity);
                _openedPool.Del(cellEntity);
                _flaggedPool.Del(cellEntity);
                _explodedPool.Del(cellEntity);

                if (!_dirtyPool.Has(cellEntity))
                    _dirtyPool.Add(cellEntity);
            }
        }

        private void NotifyAboutGameReset(EcsWorld world)
        {
            _resetFilter ??= world.Filter<GameResetListenerBind>().End();
            foreach (var e in _resetFilter)
            {
                _listeners.Get(e).Listener.GameReset();
            }
        }
    }
}