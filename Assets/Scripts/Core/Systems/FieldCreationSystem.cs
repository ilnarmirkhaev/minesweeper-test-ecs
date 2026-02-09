using Configs;
using Core.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Systems
{
    public sealed class FieldCreationSystem : IEcsInitSystem
    {
        private readonly EcsWorld _world;
        private readonly MineFieldConfig _config;
        private readonly EcsPool<CellComponent> _cellPool;

        public FieldCreationSystem(EcsWorld world, MineFieldConfig config, EcsPool<CellComponent> cellPool)
        {
            _world = world;
            _config = config;
            _cellPool = cellPool;
        }

        public void Init(IEcsSystems systems)
        {
            for (var i = 0; i < _config.Rows; i++)
            {
                for (var j = 0; j < _config.Columns; j++)
                {
                    CreateCell(i, j);
                }
            }
        }

        private void CreateCell(int x, int y)
        {
            var entity = _world.NewEntity();

            ref var cell = ref _cellPool.Add(entity);
            cell.Position = new Vector2Int(x, y);
        }
    }
}