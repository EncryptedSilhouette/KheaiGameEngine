using SFML.Graphics;

namespace KheaiGameEngine
{
    public class KWindow : KComponent
    {
        public RenderWindow window { get; private set; }

        public KWindow()
        {
            window = new(SFML.Window.VideoMode.DesktopMode, Application.AppName);
        }

        public override void Init(KApplication app)
        {
           
        }

        public override void Start()
        {
            
        }

        public override void End()
        {
           
        }

        public void Draw(object DrawableObject)
        {

        }

        public void Init(KEngine engine)
        {
            throw new NotImplementedException();
        }

        public void Init(KApplication app)
        {
            throw new NotImplementedException();
        }
    }
}
