using KheaiGameEngine;

namespace KheaiUtils
{
    public interface KObjectComponent : IKComponent
    {
        ///<summary>The active state of the component.</summary>
        public bool Enabled { get; set; }
        ///<summary>The owner of the component.</summary>
        public KGameObject Owner { get; set; }

        public void Update(uint currentTick);
        public void FrameUpdate(uint currentFrame);
    }
}