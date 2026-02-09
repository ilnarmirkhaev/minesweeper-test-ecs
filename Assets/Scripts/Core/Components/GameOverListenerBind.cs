namespace Core.Components
{
    public struct GameOverListenerBind
    {
        public IGameOverListener Listener;
    }

    public interface IGameOverListener
    {
        void GameOver(bool win);
    }
}