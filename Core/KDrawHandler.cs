using KheaiGameEngine.GameManagement;
using SFML.Graphics;

namespace KheaiGameEngine.Core
{
    public interface IKDrawable
    {
        
    }

    public class DrawData
    {
       
    }

    public class KSpriteBatch
    {
        public KSprite sprite;

        private Texture batchedTexture;
        private List<Image> textures;

        private List<KSprite> sprites = new();

        //private Thread batchThread = new(ThreadBatch);

        public KSpriteBatch()
        {

        }

        void Start() 
        {

        }

        void Draw(Image spriteTexture, KTransform transform) 
        {
            textures.Add(spriteTexture);

        }

        void SubmitSprite(KSprite sprite)
        {
            sprites.Add(sprite);
        }

        void BatchTexture() 
        { 
        }
    }

    public class KDrawHandler : KEngineComponent
    {
        protected RenderWindow window;
        protected KSceneHandler sceneHandler;
        protected KDebugger debugger;

        private SortedList<int, KSpriteBatch> _drawLayers = new();
        private VertexBuffer vertexBuffer = new(1024, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Stream);

        public KDrawHandler()
        {

        }

        public override void Init()
        {
           
        }

        public override void Start()
        {
            debugger = Engine.GetComponent<KDebugger>();
        }

        public override void Update(uint currentTick)
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

        }

        public void AddDrawComponent()
        {

        }

        public void AddReference()
        {

        }
    }
}
