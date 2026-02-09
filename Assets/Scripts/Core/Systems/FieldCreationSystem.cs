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
        private readonly EcsPool<Dirty> _dirtyPool;

        public FieldCreationSystem(EcsWorld world, MineFieldConfig config, EcsPool<CellComponent> cellPool,
            EcsPool<Dirty> dirtyPool)
        {
            _world = world;
            _config = config;
            _cellPool = cellPool;
            _dirtyPool = dirtyPool;
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

            _dirtyPool.Add(entity);
        }
    }
}