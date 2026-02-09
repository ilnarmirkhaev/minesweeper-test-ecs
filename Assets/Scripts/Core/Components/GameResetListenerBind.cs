namespace Core.Components
{
    public struct GameResetListenerBind
    {
        public IGameResetListener Listener;
    }

    public interface IGameResetListener
    {
        void GameReset();
    }
}