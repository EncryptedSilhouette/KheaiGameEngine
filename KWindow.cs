namespace KheaiGameEngine
{
    public class KWindow : KEngineComponent
    {
       
        public KWindow()
        {
            _window.SetActive(false);
        }

        public override void Init(KApplication app, KEngine engine)
        {
            _application = app;
            _application.OnEventDispatch += _window.DispatchEvents;
            _engine = engine;
            _engine.Window = _window;
        }

        public override void Start()
        {
            
        }

        public override void End()
        {
            _application.OnEventDispatch -= DispatchEvents;
        }

        public void DispatchEvents()
        {
            lock (_window)
            {
                _window.SetActive(true);
                _window.DispatchEvents();
            }
        }

        public override void FixedUpdate()
        {
           
        }

        public override void FrameUpdate()
        {
            
        }

        public void Draw(object DrawableObject)
        {

        }
    }
}
