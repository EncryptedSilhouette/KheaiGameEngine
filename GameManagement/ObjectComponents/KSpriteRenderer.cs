using KheaiGameEngine.GameObjects;
using SFML.Graphics;

namespace KheaiGameEngine.GameManagement.ObjectComponents
{
    public class KSpriteRenderer : KObjectComponent
    {
        private Sprite sprite;
        private KTransform transform;

        public string TexturePath = "DebugImage.png";

        public override void Init()
        {
            Texture texture = new(TexturePath);
            sprite = new Sprite(texture);
        }

        public override void Start()
        {
            transform = Owner.GetComponent<KTransform>();
        }

        public override void End()
        {

        }

        public override void Update(ulong currentTick)
        {
            
        }

        public override void FrameUpdate(ulong currentFrame)
        {
            sprite.Position = new(transform.Left, transform.Top);
            sprite.Rotation = transform.rotation;
        }

        public void Draw(RenderTarget target)
        {
            target.Draw(sprite);
        }
    }
}
