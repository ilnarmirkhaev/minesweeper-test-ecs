using Core.Services;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core.Systems
{
    public sealed class RestartInputSystem : IEcsRunSystem
    {
        private readonly ICellInputService _input;

        public RestartInputSystem(ICellInputService input)
        {
            _input = input;
        }

        public void Run(IEcsSystems systems)
        {
            if (Input.GetKeyDown(KeyCode.R))
                _input.RequestRestart();
        }
    }
}

