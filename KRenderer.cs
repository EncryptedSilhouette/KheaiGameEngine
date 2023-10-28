using SFML.Graphics;

namespace KheaiGameEngine
{
    public abstract class KRenderer : KEngineComponent
    {
        #region Static 
        private static KRenderer activeRenderer;
        public static KRenderer ActiveRenderer
        {
            get
            {
                return activeRenderer;
            }
            set
            {
                activeRenderer = value;
                OnRendererChange?.Invoke();
            }
        }

        //Events
        public static event KEventManager OnRendererChange;
        #endregion

        public abstract void Draw(RenderTarget target);
    }
}
