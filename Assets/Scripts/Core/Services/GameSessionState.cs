namespace Core.Services
{
    public sealed class GameSessionState
    {
        public bool GameStarted { get; set; }
        public bool IsGameOver { get; set; }
        public bool IsWin { get; set; }

        public void Reset()
        {
            GameStarted = false;
            IsGameOver = false;
            IsWin = false;
        }
    }
}