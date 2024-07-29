using KheaiGameEngine;

namespace KheaiUtils
{
    public abstract class KObjectComponent : IKComponent
    {
        ///<summary>The active state of the component.</summary>
        public bool Enabled { get; set; }
        public short Order { get; set; }
        public string ID { get; set; }

        ///<summary>The owner of the component.</summary>
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