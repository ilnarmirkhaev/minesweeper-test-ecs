using Core.Services;
using Leopotam.EcsLite;

namespace Core.Systems
{
    public class SessionResetSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly GameSessionState _session;

        public SessionResetSystem(GameSessionState session)
        {
            _session = session;
        }
        
        public void Init(IEcsSystems systems)
        {
            _session.Reset();
        }

        public void Run(IEcsSystems systems)
        {
        }
    }
}