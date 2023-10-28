using SFML.Graphics;

namespace KheaiGameEngine
{
    public class KWindow : KAppComponent
    {
        public RenderWindow Window { get; protected set; } //This will absolutely cause threading issues later, future me problem :D

        #region Logic
        public override void Init()
        {
            Window = new(SFML.Window.VideoMode.DesktopMode, App.AppName);
            Window.Closed += (x, y) => App.IsRunning = false;

            KDebug.AddLog("WINDOW");
        }

        public override void Start() 
        {
            if (KRenderer.ActiveRenderer == null)
            {
                KDebug.Log("WINDOW", "No ative renderer");
            }
        }

        public override void Update() 
        {
            DispatchEvents();
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

                KRenderer.ActiveRenderer?.Draw(Window);

                Window.Display();
                Window.SetActive(false);            
            }
        }
        #endregion

        public void DispatchEvents()
        {
            lock (this)
            {
                Window.SetActive(true);
                Window.DispatchEvents();
                Window.SetActive(false);
            }
        }
    }
}
