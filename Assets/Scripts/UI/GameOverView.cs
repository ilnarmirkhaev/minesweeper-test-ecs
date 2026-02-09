using Core.Components;
using Core.Services;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI
{
    public class GameOverView : MonoBehaviour, IGameResetListener, IGameOverListener
    {
        [SerializeField] private Text _resultText;
        [SerializeField] private Button _restartButton;

        private ICellInputService _input;
        private EcsWorld _world;

        [Inject]
        private void Inject(ICellInputService input, EcsWorld world)
        {
            _input = input;
            _world = world;

            var e = _world.NewEntity();
            ref var gameOver = ref _world.GetPool<GameOverListenerBind>().Add(e);
            gameOver.Listener = this;

            ref var gameReset = ref _world.GetPool<GameResetListenerBind>().Add(e);
            gameReset.Listener = this;
        }

        private void Start()
        {
            _restartButton.onClick.AddListener(Restart);
        }

        public void GameReset()
        {
            gameObject.SetActive(false);
        }

        public void GameOver(bool win)
        {
            gameObject.SetActive(true);
            _resultText.text = win ? "You Win :)" : "You Lose :(";
        }

        private void Restart()
        {
            _input.RequestRestart();
        }
    }
}