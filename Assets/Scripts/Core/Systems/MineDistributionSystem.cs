using System.Collections.Generic;
using Configs;
using Core.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Systems
{
    public class MineDistributionSystem : IEcsRunSystem
    {
        private readonly MineFieldConfig _config;
        private readonly EcsPool<CellComponent> _cellPool;
        private readonly EcsPool<MineComponent> _minePool;
        private readonly EcsPool<FirstCellClickedEvent> _firstCellEventPool;

        private EcsFilter _closedCellsFilter;
        private EcsFilter _firstCellClickedFilter;

        public MineDistributionSystem(MineFieldConfig config, EcsPool<CellComponent> cellPool,
            EcsPool<MineComponent> minePool, EcsPool<FirstCellClickedEvent> firstCellEventPool)
        {
            _config = config;
            _cellPool = cellPool;
            _minePool = minePool;
            _firstCellEventPool = firstCellEventPool;
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _closedCellsFilter ??= world.Filter<CellComponent>().Exc<Opened>().End();
            _firstCellClickedFilter ??= world.Filter<FirstCellClickedEvent>().End();

            var gameStart = _firstCellClickedFilter.GetEntitiesCount() > 0;
            if (gameStart) DistributeMines();
        }

        private void DistributeMines()
        {
            var candidates = new List<int>(_config.TotalCells);

            var firstCellPosition =  new Vector2Int(-1, -1);
            foreach (var entity in _firstCellClickedFilter)
            {
                firstCellPosition = _firstCellEventPool.Get(entity).Position;
            }

            foreach (var entity in _closedCellsFilter)
            {
                if (_cellPool.Get(entity).Position == firstCellPosition) continue;
                candidates.Add(entity);
            }

            Shuffle(candidates);

            var count = Mathf.Min(_config.MinesCount, candidates.Count);
            for (var i = 0; i < count; i++)
            {
                _minePool.Add(candidates[i]);
            }
        }

        private static void Shuffle(List<int> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}