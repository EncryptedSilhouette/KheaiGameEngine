using SFML.Graphics;

namespace KheaiGameEngine
{
    #region KRenderer

    public interface IKDrawHandler
    {
        ///<summary>Defines the behavior for drawing.</summary>
        public void Render(RenderTarget target);
    }

    public static class KRenderer
    {
        private static IKDrawHandler s_activeInstance;

        ///<summary>Event handler for when the active instance is changed</summary>
        public static event Action OnInstanceChanged;

        ///<summary>Property for the static refrence to the active instance. Fires an event when the instance is changed.</summary>
        public static IKDrawHandler ActiveInstance
        {
            get => s_activeInstance;
            set 
            {
                OnInstanceChanged?.Invoke();
                s_activeInstance = value;
            }
        }

        static KRenderer() => s_activeInstance = new KStandardRenderer(new());

        ///<summary>Static method to render a frame using the active KRenderer instance.</summary>
        public static void RenderFrame(RenderTarget target) => s_activeInstance.Render(target);
    }

    #endregion

    #region KStandardRenderer

    public class KStandardRenderer : KEngineComponent, IKDrawHandler
    {
        private VertexArray _vertexArray;

        public RenderStates RenderStates { get; set; }

        public KStandardRenderer(RenderStates renderStates) => RenderStates = renderStates;

        public override void Init() { }
        public override void Start() { }
        public override void End() { }
        public override void Update(uint currentUpdate) { }
        public override void FrameUpdate(uint currentUpdate) { }

        public void Render(RenderTarget target)
        {
            target.Draw(_vertexArray, RenderStates);
            _vertexArray.Clear();
        }
        
        public void Draw(ref Vertex[] vertices) 
        {
            foreach (var vertex in vertices) _vertexArray.Append(vertex);
        }
    }

#endregion
}




