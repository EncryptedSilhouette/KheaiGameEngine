using KheaiGameEngine.GameManagement;
using SFML.Graphics;

namespace KheaiGameEngine.Core
{
    public interface IKDrawable
    {
        public short Layer { get; protected set; }
        public Image texture { get; protected set; }
        public Vertex[] vertices { get; protected set; }
    }

    public class KSpriteBatch
    {
        public Vertex[] Vertices { get; private set; }
        public Texture CombinedTexture { get; private set; }

        private Queue<Image> images = new();
        private List<Vertex> vertices = new();

        private Texture texture = new(1024, 1024);
        Dictionary<uint, uint> offsets = new();

        Thread batchThread;

        public KSpriteBatch() 
        {
            batchThread = new(Batch);
        }

        void Start()
        {
            texture = new(1024, 1024);
        }

        void Draw(IKDrawable drawable)
        {
            
        }

        void End()
        {

        }

        void Batch() 
        {
            Image image;
            uint offsetY = 0;
            uint offsetX = 0;

            lock (images) image = images.Dequeue();
            for (uint i = 4; i < image.Size.Y; i *= 2) offsetY = i;

            if (offsets.ContainsKey(offsetY)) offsetX = offsets[offsetY];
            else offsets[offsetY] = 0;

            lock (texture) texture.Update(image, offsetX, offsetY);
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
            foreach (var layer in drawables)
            {
                foreach (var drawable in layer.Value)
                {
                    target
                }
            }
        }

        public void AddDrawComponent()
        {

        }

        public void AddReference()
        {

        }
    }
}
