using Core.Components;
using Leopotam.EcsLite;
using UI;

namespace Core.Systems
{
    public sealed class CellViewDrawSystem : IEcsRunSystem
    {
        private readonly ICellViewRegistry _views;

        private readonly EcsPool<CellComponent> _cellPool;
        private readonly EcsPool<Opened> _openedPool;
        private readonly EcsPool<Flagged> _flaggedPool;
        private readonly EcsPool<Exploded> _explodedPool;
        private readonly EcsPool<NeighborMinesCount> _neighborPool;
        private readonly EcsPool<Dirty> _dirtyPool;

        private EcsFilter _filter;

        public CellViewDrawSystem(ICellViewRegistry views, EcsPool<CellComponent> cellPool, EcsPool<Opened> openedPool,
            EcsPool<Flagged> flaggedPool, EcsPool<Exploded> explodedPool, EcsPool<NeighborMinesCount> neighborPool,
            EcsPool<Dirty> dirtyPool)
        {
            _views = views;
            _cellPool = cellPool;
            _openedPool = openedPool;
            _flaggedPool = flaggedPool;
            _explodedPool = explodedPool;
            _neighborPool = neighborPool;
            _dirtyPool = dirtyPool;
        }

        public void Run(IEcsSystems systems)
        {
            _filter ??= systems.GetWorld().Filter<CellComponent>().Inc<Dirty>().End();
            foreach (var e in _filter)
            {
                ref var cell = ref _cellPool.Get(e);
                if (_views.TryGet(cell.Position, out var view))
                {
                    var state = ResolveState(e);
                    var neighborCount = state == CellVisualState.Opened && _neighborPool.Has(e)
                        ? _neighborPool.Get(e).Value
                        : (int?)null;
                    view.SetVisual(state, neighborCount);
                }

                _dirtyPool.Del(e);
            }
        }

        private CellVisualState ResolveState(int entity)
        {
            if (_explodedPool.Has(entity))
                return CellVisualState.Exploded;
            if (_flaggedPool.Has(entity))
                return CellVisualState.Flagged;
            if (_openedPool.Has(entity))
                return CellVisualState.Opened;
            return CellVisualState.Closed;
        }
    }
}