
using KheaiGameEngine.Core;
using SFML.Graphics;

namespace KheaiGameEngine.DevDebug
{
    public interface IKRenderer
    {
        public int DrawLayer { get; set; }
        Drawable[] drawables { get; set; }
    }

    public class KRenderManager : KEngineComponent
    {
        private Queue<IKRenderer> addRenderers = new();
        private Queue<IKRenderer> removeRenderers = new();
        private Dictionary<int, List<IKRenderer>> _renderLayers = new();

        public override void Init()
        {
            Order = ushort.MaxValue - 10;
        }

        public override void Start()
        {
            
        }

        public override void End()
        {

        }

        public override void Update(ulong currentTick)
        {
            
        }

        public override void FrameUpdate(ulong currentFrame)
        {
            while (removeRenderers.Count > 0)
            {
                IKRenderer renderer = removeRenderers.Dequeue();
                _renderLayers[renderer.DrawLayer].Remove(renderer);
            }
            while (addRenderers.Count > 0)
            {
                IKRenderer renderer = addRenderers.Dequeue();
                _renderLayers[renderer.DrawLayer].Add(renderer);
            }
        }

        public void Draw(RenderTarget target)
        {
            foreach (var renderLayer in _renderLayers.Values)
            {
                foreach (var renderer in renderLayer)
                {
                    
                }
            }
        }

        public void AddRenderer(IKRenderer renderer)
        {
            _renderLayers[renderer.DrawLayer].Add(renderer);
        }

        public void RemoveRenderer(IKRenderer renderer)
        {
            _renderLayers[renderer.DrawLayer].Remove(renderer);
        }
    }
}