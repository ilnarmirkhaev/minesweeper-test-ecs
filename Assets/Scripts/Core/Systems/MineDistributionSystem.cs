using Configs;
using Core.Components;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public class MineDistributionSystem : IEcsInitSystem
    {
        private readonly MineFieldConfig _config;
        private readonly EcsPool<MineComponent> _minePool;

        private EcsFilter _closedCellsFilter;

        public MineDistributionSystem(MineFieldConfig config, EcsPool<MineComponent> minePool)
        {
            _config = config;
            _minePool = minePool;
        }
        
        public void Init(IEcsSystems systems)
        {
            _closedCellsFilter = systems.GetWorld().Filter<CellComponent>().Exc<Opened>().End();
            
            DistributeMinesAfterFirstOpenCell();
        }

        private void DistributeMinesAfterFirstOpenCell()
        {
            var cellsLeft = _config.Rows * _config.Columns - 1;
            var minesLeft = _config.MinesCount;

            foreach (var cell in _closedCellsFilter)
            {
                ref var mine = ref _minePool.Add(cell);
                cellsLeft--;
                minesLeft--;
            }
        }
    }
}