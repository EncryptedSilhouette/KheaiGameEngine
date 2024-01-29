using KheaiGameEngine.Core;
using SFML.Graphics;

namespace KheaiGameEngine.DevDebug
{
    public class DebugRenderer
    {
        KEngine _engine;
        Texture _texture;
        Sprite _sprite;
        Text _debugText;

        public DebugRenderer(KEngine engine)
        {
            _engine = engine;
            _texture = new("Res\\DebugImage.png");

            _sprite = new Sprite(_texture);
            _sprite.Position = new(50, 50);

            _debugText = new Text();
            _debugText.Font = new();
            _debugText.Position = new(100, 100);
            _debugText.Color = Color.White; 
            _debugText.DisplayedString = $"CurrentTick:{engine.CurrentTick.ToString()}" +
                                         $"\nUPS: {engine.UpdateRate} "; 
        }

        public void Draw(RenderTarget renderer)
        {
            renderer.Draw(_sprite);
            renderer.Draw(_debugText);
        }
    }
}
