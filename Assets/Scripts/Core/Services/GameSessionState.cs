using Configs;

namespace Core.Services
{
    /// <summary>
    /// Lightweight session state (not per-cell). Cell state is still stored in ECS components.
    /// </summary>
    public sealed class GameSessionState
    {
        public bool GameStarted { get; set; }
        public bool IsGameOver { get; set; }
        public bool IsWin { get; set; }
        public int RemainingFlags { get; set; }

        private readonly MineFieldConfig _config;

        public GameSessionState(MineFieldConfig config)
        {
            _config = config;
        }

        public void Reset()
        {
            GameStarted = false;
            IsGameOver = false;
            IsWin = false;
            RemainingFlags = _config != null ? _config.MinesCount : 0;
        }
    }
}

