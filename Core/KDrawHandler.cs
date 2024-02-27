using KheaiGameEngine.ObjectComponents;
using SFML.Graphics;

namespace KheaiGameEngine.Core
{
    public class KDrawHandler : KEngineComponent
    {
        protected RenderWindow window;
        protected KSceneHandler sceneHandler;
        protected KDebugger debugger;

        public SortedList<int, IKDrawable> drawables = new();

        public KDrawHandler()
        {

        }

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

        public void Draw()
        {

        }

        public void AddDrawComponent()
        {

        }
    }
}
