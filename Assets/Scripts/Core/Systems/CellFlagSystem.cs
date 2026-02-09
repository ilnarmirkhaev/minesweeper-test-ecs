using Core.Components;
using Core.Services;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public sealed class CellFlagSystem : IEcsRunSystem
    {
        private readonly ICellLookup _lookup;
        private readonly GameSessionState _session;

        private readonly EcsPool<Opened> _openedPool;
        private readonly EcsPool<Flagged> _flaggedPool;
        private readonly EcsPool<Dirty> _dirtyPool;
        private readonly EcsPool<ToggleFlagRequest> _requestPool;
        
        private EcsFilter _requestFilter;

        public CellFlagSystem(ICellLookup lookup, GameSessionState session, EcsPool<Opened> openedPool,
            EcsPool<Flagged> flaggedPool, EcsPool<Dirty> dirtyPool, EcsPool<ToggleFlagRequest> requestPool)
        {
            _lookup = lookup;
            _session = session;
            _openedPool = openedPool;
            _flaggedPool = flaggedPool;
            _dirtyPool = dirtyPool;
            _requestPool = requestPool;
        }

        public void Run(IEcsSystems systems)
        {
            if (_session.IsGameOver) return;

            _requestFilter ??= systems.GetWorld().Filter<ToggleFlagRequest>().End();

            foreach (var reqEntity in _requestFilter)
            {
                ref var req = ref _requestPool.Get(reqEntity);

                if (!_lookup.TryGetCellEntity(req.Position, out var cellEntity) || _openedPool.Has(cellEntity))
                {
                    systems.GetWorld().DelEntity(reqEntity);
                    continue;
                }

                ToggleFlag(cellEntity);

                MarkDirty(cellEntity);
                systems.GetWorld().DelEntity(reqEntity);
            }
        }

        private void ToggleFlag(int cellEntity)
        {
            if (_flaggedPool.Has(cellEntity))
            {
                _flaggedPool.Del(cellEntity);
            }
            else
            {
                _flaggedPool.Add(cellEntity);
            }
        }

        private void MarkDirty(int entity)
        {
            if (!_dirtyPool.Has(entity))
                _dirtyPool.Add(entity);
        }
    }
}
