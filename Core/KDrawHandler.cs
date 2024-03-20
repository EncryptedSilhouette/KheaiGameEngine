using KheaiGameEngine.GameManagement;
using KheaiGameEngine.GameObjects;
using SFML.Graphics;

namespace KheaiGameEngine.Core
{
    public class KSprite
    {
        public Vertex[] vertices { get; private set; }

        private KTransform transform;
        
    }

    public class KSpriteManager : KObjectComponent
    {
        public List<KSprite> sprites = new();

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void End()
        {
            throw new NotImplementedException();
        }

        public override void Update(uint currentTick)
        {
            throw new NotImplementedException();
        }

        public override void FrameUpdate(uint currentFrame)
        {
            throw new NotImplementedException();
        }
    }

    public class KSpriteBatch
    {
        //Batching stuff
        Texture batchedtexture;
        VertexBuffer vertexBuffer;

        void BatchTexture() 
        {
            //Add Texture to texture batch
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
