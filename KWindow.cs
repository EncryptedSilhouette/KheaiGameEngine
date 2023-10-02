using SFML.Graphics;

namespace KheaiGameEngine
{
    public interface IKRenderer
    {
        void Draw(RenderTarget target);
        Drawable[] SubmitDraw();
    }

    public class KWindow : KComponent
    {
        public RenderWindow Window { get; protected set; } //This will absolutely cause threading issues later, future me problem :D
        public IKRenderer ActiveRenderer { get; protected set; }

        //Events
        event KEventManager onActiveRendererChange;

        #region Logic
        public override void Init()
        {
            Window = new(SFML.Window.VideoMode.DesktopMode, Owner.AppName);
            Window.Closed += (x, y) => KApplication.End();

            KApplication.OnEventDispatch += DispatchEvents;
        }

        public override void End()
        {
            lock (this)
            {
                Window.SetActive(true);
                Window.Close();
                Window.SetActive(false);
            }
        }

        public void Draw()
        {
            lock (this)
            {
                Window.SetActive(true);
                Window.Clear();

                ActiveRenderer?.Draw(Window);

                Window.Display();
                Window.SetActive(false);            
            }
        }
        #endregion

        public void SetActiveRenderer(IKRenderer renderer)
        {
            lock (this)
            {
                ActiveRenderer = renderer;
                onActiveRendererChange?.Invoke();
            }
        }

        public void DispatchEvents()
        {
            lock (this)
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
