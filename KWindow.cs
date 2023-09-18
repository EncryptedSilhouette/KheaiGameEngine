using SFML.Graphics;

namespace KheaiGameEngine
{
    public interface IKRenderer
    {
        void Draw(RenderTarget target);
    }

    public interface IKRenderComponent
    {
        Drawable[] SubmitDraw();
    }

    public class KRenderer : IKRenderer
    {
        private List<IKRenderComponent> _kRenderComponents = new();

        public void Draw(RenderTarget target)
        {
            foreach (IKRenderComponent renderer in _kRenderComponents)
            {
                foreach (Drawable drawable in renderer.SubmitDraw())
                {
                    target.Draw(drawable);
                }               
            }
        }
    }

    //make a renderer component or something dumbass idefk what youre doing here
    public class KWindow : KAppComponent
    {
        #region Static
        public static IKRenderer activeRenderer { get; private set; }

        //events
        public static event KEventManager ActiveRendererChanged;

        public static void setActiveRenderer(IKRenderer renderer)
        {
            activeRenderer = renderer;
            ActiveRendererChanged.Invoke();
        }
        #endregion

        //This will absolutely cause threading issues later, future me problem :D
        public RenderWindow Window { get; protected set; }

        public override void Init()
        {
            Window = new(SFML.Window.VideoMode.DesktopMode, Application.AppName);
            Window.Closed += (ignore0, ignore1) => Application.End();

            Application.OnEventDispatch += DispatchEvents;
        }

        public override void End()
        {
            lock (Window)
            {
                Window.SetActive(true);
                Window.Close();
                Window.SetActive(false);
            }
        }

        public void Draw()
        {
            lock (Window)
            {
                Window.SetActive(true);
                Window.Clear();

                activeRenderer?.Draw(Window);

                Window.Display();
                Window.SetActive(false);            
            }
        }

        public void DispatchEvents()
        {
            lock (Window)
            {
                Window.SetActive(true);
                Window.DispatchEvents();
                Window.SetActive(false);
            }
        }

        #region Ignored
        public override void Start() { }
        #endregion
    }
}
