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
        private readonly EcsPool<MineComponent> _minePool;

        private EcsFilter _closedCellsFilter;
        private EcsFilter _gameStartedFilter;

        public MineDistributionSystem(MineFieldConfig config, EcsPool<MineComponent> minePool)
        {
            _config = config;
            _minePool = minePool;
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _closedCellsFilter ??= world.Filter<CellComponent>().Exc<Opened>().End();
            _gameStartedFilter ??= world.Filter<FirstCellOpenedEvent>().End();

            var gameStart = _gameStartedFilter.GetEntitiesCount() > 0;
            if (gameStart) DistributeMines();
        }

        private void DistributeMines()
        {
            var candidates = new List<int>(_config.TotalCells);

            foreach (var entity in _closedCellsFilter)
                candidates.Add(entity);

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