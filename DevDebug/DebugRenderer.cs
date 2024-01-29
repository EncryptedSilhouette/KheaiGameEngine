using KheaiGameEngine.Core;
using SFML.Graphics;

namespace KheaiGameEngine.DevDebug
{
    public class DebugRenderer
    {
        KEngine _engine;
        Texture _texture;
        Text _debugText;

        public DebugRenderer(KEngine engine)
        {
            _engine = engine;
            _texture = new("res\\debug.png");
            _debugText = new Text();
            _debugText.Font = new("res\\font.ttf");
            _debugText.Position = new(0, 0);
            _debugText.Color = Color.White; 
        }

        public void Draw(RenderTarget renderer)
        {
            _debugText.DisplayedString = $"CurrentTick:{_engine.CurrentTick.ToString()}" +
                                         $"\nUPS: {_engine.UpdateRate} ";
            renderer.Draw(_debugText);
        }
    }
}
