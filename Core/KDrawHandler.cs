using SFML.Graphics;
using SFML.System;

namespace KheaiGameEngine
{
    public interface IKRenderer 
    {
        public void Draw(RenderTarget target);
    }

    public class SpriteBatch 
    {
        static Texture textureAtlas;

        RenderStates renderStates;
        List<Queue<string>> Layers = new();
        Dictionary<string, Vector2f[]> textureLookup = new();

        VertexBuffer _vertexBuffer = new(4 * 128, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Stream);

        Vector2f[] this[string textureID] => textureLookup[textureID];

        Queue<Vertex> vertices = new();

        void SubmitDraw(string TexID, int x, int y, uint width, uint height, uint layer) 
        {
            
        }

        void SubmitDraw(string TexID, Vector2f vertexA, Vector2f vertexB, uint layer)
        {

        }

        void SubmitDraw(string TexID, Vertex[] vertex, uint layer)
        {

        }

        void Draw() 
        {
      
        }

        public void Thread() 
        {

        }
    }

    public abstract class KDrawHandler : KEngineComponent, IKRenderer
    {
        protected RenderWindow window;
        protected KSceneHandler sceneHandler;

        public KDrawHandler() 
        {

        }

        public override void Init()
        {

        }

        public override void Start()
        {

        }

        public override void Update(uint currentFrame)
        {

        }

        public override void FrameUpdate(uint currentFrame)
        {

        }

        public override void End()
        {

        }

        public void Draw(RenderTarget target)
        {
            window.Clear(Color.Black);

            window.Display();
        }

        public void AddDrawComponent()
        {
         
        }
    }
}
