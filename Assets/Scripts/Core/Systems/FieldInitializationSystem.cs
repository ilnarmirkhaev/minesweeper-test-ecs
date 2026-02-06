using Configs;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public class FieldInitializationSystem : IEcsInitSystem
    {
        private readonly MineFieldConfig _config;

        public FieldInitializationSystem(MineFieldConfig config)
        {
            _config = config;
        }

        public void Init(IEcsSystems systems)
        {
        }
    }
}