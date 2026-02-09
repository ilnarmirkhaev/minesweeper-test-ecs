using Core.Components;
using Core.Services;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public sealed class RestartSystem : IEcsRunSystem
    {
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<NeighborMinesCount> _neighborPool;
        private readonly EcsPool<Opened> _openedPool;
        private readonly EcsPool<Flagged> _flaggedPool;
        private readonly EcsPool<Exploded> _explodedPool;
        private readonly EcsPool<Dirty> _dirtyPool;
        private readonly GameSessionState _session;

        public RestartSystem(EcsPool<MineComponent> minePool, EcsPool<NeighborMinesCount> neighborPool,
            EcsPool<Opened> openedPool, EcsPool<Flagged> flaggedPool, EcsPool<Exploded> explodedPool,
            EcsPool<Dirty> dirtyPool, GameSessionState session)
        {
            _minePool = minePool;
            _neighborPool = neighborPool;
            _openedPool = openedPool;
            _flaggedPool = flaggedPool;
            _explodedPool = explodedPool;
            _dirtyPool = dirtyPool;
            _session = session;
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var restartFilter = world.Filter<RestartRequest>().End();
            var any = false;
            foreach (var reqEntity in restartFilter)
            {
                any = true;
                world.DelEntity(reqEntity);
            }

            if (!any) return;

            _session.Reset();

            var allCellsFilter = world.Filter<CellComponent>().End();
            foreach (var cellEntity in allCellsFilter)
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
    }
}

