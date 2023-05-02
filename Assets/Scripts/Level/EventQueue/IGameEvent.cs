namespace Level.EventQueue {
    public interface IGameEvent {
        public GameEventType GetType();
        public void Enqueue(float delay);
        public bool CanReleased();
        public void Release();
        public float GetDelay();
    }
}
