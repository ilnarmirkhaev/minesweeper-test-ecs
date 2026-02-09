using System.Collections.Generic;
using System.Text;
using Configs;
using Core.Components;
using Core.Services;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Systems
{
    public class DebugFieldLogSystem : IEcsRunSystem
    {
        private readonly MineFieldConfig _config;
        private readonly ICellLookup _lookup;
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<NeighborMinesCount> _neighborCountPool;

        private EcsFilter _filter;

        public DebugFieldLogSystem(MineFieldConfig config, ICellLookup lookup,
            EcsPool<MineComponent> minePool, EcsPool<NeighborMinesCount> neighborCountPool)
        {
            _config = config;
            _lookup = lookup;
            _minePool = minePool;
            _neighborCountPool = neighborCountPool;
        }

        public void Run(IEcsSystems systems)
        {
            _filter ??= systems.GetWorld().Filter<FirstCellClickedEvent>().End();

            if (_filter.GetEntitiesCount() <= 0) return;

            var sb = new StringBuilder();
            for (var row = 0; row < _config.Rows; row++)
            {
                for (var col = 0; col < _config.Columns; col++)
                {
                    var pos = new Vector2Int(row, col);
                    if (!_lookup.TryGetCellEntity(pos, out var e))
                    {
                        sb.Append('?');
                        continue;
                    }

                    sb.Append(_minePool.Has(e) ? 'X' : (char)('0' + _neighborCountPool.Get(e).Value));
                }

                sb.AppendLine();
            }

            Debug.Log("[Minesweeper Field]\n" + sb);
        }
    }
}