using KheaiGameEngine.Core;
using KheaiGameEngine.GameObjects;
using SFML.Graphics;

namespace KheaiGameEngine.GameManagement
{
    public interface IKDrawableObject
    {
        KTransform transform { get; set; }

        void SubmitDraw();
    }

    public class KSprite : KObjectComponent, IKDrawableObject
    {
        public Texture texture { get; set; }    
        public KTransform transform { get; set; }

        public override void Init() { }
        public override void Start() { }
        public override void End() { }
        public override void Update(uint currentTick) { }
        public override void FrameUpdate(uint currentFrame) { }

        public void SubmitDraw() 
        {

        }
    }
}
