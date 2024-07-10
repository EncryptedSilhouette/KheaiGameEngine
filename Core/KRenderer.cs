using SFML.Graphics;

namespace KheaiGameEngine
{
    public interface IKDrawHandeler 
    {
        ///<summary>Defines the behavior for drawing.</summary>
        public void Render(RenderTarget target);
    }

    #region KRenderer

    public static class KRenderer
    {
        private static IKDrawHandeler s_activeInstance;

        ///<summary>Event handler for when the active instance is changed</summary>
        public static event Action OnInstanceChanged;

        ///<summary>Property for the static refrence to the active instance. Fires an event when the instance is changed.</summary>
        public static IKDrawHandeler ActiveInstance
        {
            get => s_activeInstance;
            set
            {
                OnInstanceChanged?.Invoke();
                s_activeInstance = value;
            }
        }

        ///<summary>Static method to render a frame using the active KRenderer instance.</summary>
        public static void RenderFrame(RenderTarget target) => s_activeInstance.Render(target);
    }

    #endregion


    public class TextureAtlas 
    {
        List
    }

    public class Sprite 
    {
        public Sprite() 
        {

        }
    }

    public class SpriteBatch 
    {
        int verticies;
        Texture textureAtlas; 
        VertexBuffer VertexBuffer;

        public SpriteBatch()
        {

        }

        public void StartBatch() 
        {

        }

        public void Draw(Vertex[] vertices, Texture texture) 
        {

        }

        public void EndBatch()
        {

        }
    }

    public class KStandardRenderer : KRenderer
    {
        RenderStates renderStates;
        VertexBuffer vertexBuffer;

        List<(int order, string taskID, Action task)> _preRenderTasks = new();
        List<(int order, string taskID, Action task)> _postRenderTasks = new();

        public KStandardRenderer(uint vertexCount)
        {
            vertexBuffer = new(vertexCount, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Stream);
        }

        public void Render(RenderTarget target)
        {
           
        }

        public void AddPreRenderTask(int order, string taskID, Action task) 
        {
            _preRenderTasks.Add((order, taskID, task));
            _preRenderTasks.Sort((a, b) => 
            {
                if (a.order > b.order) return 1;
                if (a.order < b.order) return -1;
                return 0;
            });
        }

        public void AddPostRenderTask(int order, string taskID, Action task)
        {
            _postRenderTasks.Add((order, taskID, task));
            _postRenderTasks.Sort((a, b) =>
            {
                if (a.order > b.order) return 1;
                if (a.order < b.order) return -1;
                return 0;
            });
        }

        public void RemovePreRenderTask(string taskID) => 
            _preRenderTasks.RemoveAt(
                _preRenderTasks.FindIndex((task) => (task.taskID == taskID) ? true : false));

        public void RemovePostRenderTask(string taskID) =>
            _postRenderTasks.RemoveAt(
                _postRenderTasks.FindIndex((task) => (task.taskID == taskID) ? true : false));
    }
}




