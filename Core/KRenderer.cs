using SFML.Graphics;

namespace KheaiGameEngine
{
    public class KDrawArgs : EventArgs
    {
        public RenderStates RenderStates;
    }

    public class KRenderer
    {
        #region Staic

        public static event EventHandler<KRenderer> OnInstanceChanged;

        private static KRenderer _activeInstance;

        public static void RenderFrame(RenderTarget target)
        {

        }

        #endregion

        public KDrawArgs drawArgs;

        public event EventHandler<KDrawArgs> onPreDraw;
        public event EventHandler<KDrawArgs> onDraw;
        public event EventHandler<KDrawArgs> onPostDraw;

        public KRenderer ActiveInstance 
        {
            get => _activeInstance;
            set 
            {
                OnInstanceChanged?.Invoke(this, value);
                _activeInstance = value;
            }
        }

        public void Draw(RenderTarget renderTarget) 
        {
            onPreDraw.Invoke(this, drawArgs);
            onDraw.Invoke(this, drawArgs);
            onPostDraw.Invoke(this, drawArgs);
        }
    }
}
