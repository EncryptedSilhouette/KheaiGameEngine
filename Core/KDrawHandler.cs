using SFML.Graphics;
using SFML.Window;
using System.Text;

namespace KheaiGameEngine.Core
{
    public class KDrawHandler : KEngineComponent
    {
        protected RenderWindow window;
        protected KSceneHandler sceneHandler;
        protected Text text;
        protected KDebugger debugger;

        protected StringBuilder stringBuilder = new();

        public KDrawHandler()
        {

        }

        public override void Init()
        {
            window = Engine.Window;
            text = new Text();
            text.Position = new(50, 50);
            text.Font = new("res\\font.ttf");

            window.TextEntered += Window_TextEntered;
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
            text.DisplayedString = stringBuilder.ToString();
        }

        public override void End()
        {
           
        }

        public void Draw()
        {
            window.Draw(text);
        }

        private void Window_TextEntered(object sender, TextEventArgs e)
        {
            stringBuilder.Append(e.Unicode);
        }
    }
}
