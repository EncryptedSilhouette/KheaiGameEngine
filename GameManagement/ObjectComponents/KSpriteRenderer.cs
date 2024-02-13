using KheaiGameEngine.GameObjects;
using SFML.Graphics;

namespace KheaiGameEngine.ObjectComponents
{
    public interface IKDrawable 
    {
        public short Layer { get; protected set; }
        public Drawable Drawable { get; protected set; }

        public void Draw(RenderTarget target); 
    }

    public class KSpriteRenderer : KObjectComponent, IKDrawable
    {
        private Sprite _sprite;
        private KTransform _transform;

        public short Layer { get; set; }
        public Drawable Drawable { get; set; }
        public string TexturePath { get; set; } = "DebugImage.png";

        public override void Init()
        {
            Texture texture = new(TexturePath);
            _sprite = new Sprite(texture);
        }

        public override void Start()
        {
            _transform = Owner.GetComponent<KTransform>();
        }

        public override void End()
        {

        }

        public override void Update(uint currentTick)
        {
            
        }

        public override void FrameUpdate(uint currentFrame)
        {
            _sprite.Position = new(_transform.Left, _transform.Top);
            _sprite.Rotation = _transform.rotation;
        }

        public void Draw(RenderTarget target)
        {
            target.Draw(_sprite);
        }
    }
}
