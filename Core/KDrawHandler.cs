using SFML.Graphics;

namespace KheaiGameEngine.Core
{

    public class KDrawHandler : KEngineComponent
    {
        protected RenderWindow window;
        protected KSceneHandler sceneHandler;
        protected KDebugger debugger;

        public override void Init()
        {
           
        }

        public override void Start()
        {
            debugger = Engine.GetComponent<KDebugger>();
        }

        public override void Update(uint currentTick)
        {

        }

        public override void FrameUpdate(uint currentFrame)
        {

        }

        public override void End()
        {

        }

        public void Draw(RenderTarget target)
        {
            
        }

        public void AddDrawComponent()
        {

        }

        public void AddReference()
        {

        }
    }
}
