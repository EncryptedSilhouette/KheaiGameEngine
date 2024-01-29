using KheaiGameEngine.Core;
using SFML.Graphics;

namespace KheaiGameEngine.DevDebug
{
    public interface IKRederer
    {
        Drawable[] drawables { get; set; }
    }

    public class KRenderManager : KEngineComponent
    {
        List<IKRederer> renderers = new(); 

        public override void Init()
        {

        }

        public override void Start()
        {
            
        }

        public override void End()
        {

        }

        public override void Update(ulong currentTick)
        {
            
        }

        public override void FrameUpdate(ulong currentFrame)
        {

        }

        public void Draw(RenderTarget target)
        {

        }
    }
}
