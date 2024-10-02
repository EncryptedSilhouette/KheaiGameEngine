#if DEBUG
using KheaiGameEngine.Core;
using SFML.Graphics;
using SFML.Window;

namespace KheaiGameEngine.wip
{
    public class KWindow : IKEngineObject, IKRenderer
    {
        private RenderWindow _window;
        private KEngine _engine;

        //Implemented from IKEngineObject
        public int Order => 0;
        public bool Enabled { get; }
        public string ID { get; }

        public event Action<KWindow>? OnPreDraw;
        public event Action<KWindow>? OnPostDraw;

        public string Title { get; private set; }

        public KWindow(KEngine engine, VideoMode videoMode, string title)
        {
            ID = GetType().Name;
            Title = title;
            _engine = engine;
            _window = new(videoMode, title);
            _window.Closed += (i1, i2) => _window.Close();
            _window.Closed += (i1, i2) => _engine.Stop();
        }

        public void Start()
        {

        }

        public void End()
        {

        }

        public void Update(uint currentUpdate) => _window.DispatchEvents();

        public void FrameUpdate(uint currentUpdate)
        {
            
        }

        public void RenderFrame()
        {
            OnPreDraw?.Invoke(this);
            _window.Clear();
            //Render
            _window.Display();
            OnPostDraw?.Invoke(this);
        }
    }
}
#endif