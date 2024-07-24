namespace KheaiGameEngine.GameManagement
{
    public abstract class KObjectComponent : IKComponent
    {
        public bool Enabled { get; set; }
        public ushort Order { get; set; }
        public string ID { get; set; }
        public KGameObject Owner { get; set; }

        public KObjectComponent()
        {
            ID = GetType().Name;
            Enabled = true;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        public abstract void Update(uint currentTick);
        public abstract void FrameUpdate(uint currentFrame);
    }
}