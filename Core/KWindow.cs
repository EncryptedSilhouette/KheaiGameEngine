using SFML.Graphics;
using SFML.Window;

namespace KheaiGameEngine.Core
{
    public class KWindow : IKObject, IKRenderer
    {
        private RenderWindow _window;

        public bool Enabled { get; }

        public bool IsUnique => true;

        public int Order => 0;

        public string ID { get; } 

        public string Title { get; } = string.Empty;

        public IKObject Parent { get;  }

        public KWindow(string title) 
        {
            (_window, ID) = (new(VideoMode.DesktopMode, Title), GetType().Name);
            _window.Closed += (i1, i2) => _window.Close();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Update(uint currentUpdate)
        {
            throw new NotImplementedException();
        }

        public void FrameUpdate(uint currentUpdate)
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public void RenderFrame()
        {
            throw new NotImplementedException();
        }
    }
}
