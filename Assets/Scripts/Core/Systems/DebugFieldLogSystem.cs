using System.Collections.Generic;
using System.Text;
using Configs;
using Core.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Systems
{
    public class DebugFieldLogSystem : IEcsInitSystem
    {
        private readonly MineFieldConfig _config;
        private readonly EcsPool<CellComponent> _cellPool;
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<NeighborMinesCount> _neighborCountPool;

        public DebugFieldLogSystem(MineFieldConfig config, EcsPool<CellComponent> cellPool,
            EcsPool<MineComponent> minePool, EcsPool<NeighborMinesCount> neighborCountPool)
        {
            _config = config;
            _cellPool = cellPool;
            _minePool = minePool;
            _neighborCountPool = neighborCountPool;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<CellComponent>().End();

            var positionToEntity = new Dictionary<Vector2Int, int>();
            foreach (var entity in filter)
            {
                ref var cell = ref _cellPool.Get(entity);
                positionToEntity[cell.Position] = entity;
            }

            var sb = new StringBuilder();
            for (var row = 0; row < _config.Rows; row++)
            {
                for (var col = 0; col < _config.Columns; col++)
                {
                    var pos = new Vector2Int(row, col);
                    if (!positionToEntity.TryGetValue(pos, out var e))
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