using KheaiGameEngine.Core;
using SFML.Graphics;

namespace KheaiGameEngine
{
    public class KSpritePrefab 
    {
        Texture texture;
        VertexArray array;
        List<KTransform> transforms = new();

    }

    public class KSpriteCluster 
    {

    }

    public class SpriteBatch 
    {
        Texture texture;
        Drawable drawable;
        RenderStates state;

        public SpriteBatch(Texture texture, string id) 
        {

        }

        public void Update() 
        {

        }

        public void Draw(RenderTarget target)
        {
            target.Draw(drawable, state);
        }
    }

    public abstract class KDrawHandler : KEngineComponent
    {
       
    }
}
