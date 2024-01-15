using SFML.Graphics;

namespace KheaiGameEngine
{
    public class KSpriteRenderer : KObjectComponent
    {

        protected Drawable sprite;
        protected KSceneRenderer renderer;

        private int _layer = 0;

        public int Layer
        {
            get => _layer;
            set
            {
            
                if (_layer != value) { }
                _layer = value;
            }
        }

        public override void Init()
        {
            CircleShape image = new(64);
            image.FillColor = Color.Green;
            image.Position = new(100, 100);

            sprite = image;
        }

        public override void Start()
        {
            renderer = Owner.SceneManager.Engine.GetComponent<KSceneRenderer>();
            //renderer.drawables.Add(sprite);
        }

        public override void FixedUpdate()
        {

        }

        public override void FrameUpdate(double deltaTIme)
        {

        }

        public override void End()
        {

        }

        public void Draw(RenderTarget target)
        {
            target.Draw(sprite);
        }
    }
}
