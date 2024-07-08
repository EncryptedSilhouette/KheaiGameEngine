using SFML.Graphics;

namespace KheaiGameEngine
{
    public abstract class KRenderer
    {
        #region Static

        private static KRenderer s_activeInstance;

        public static event Action OnInstanceChanged;

        public static KRenderer ActiveInstance
        {
            get => s_activeInstance;
            set
            {
                OnInstanceChanged?.Invoke();
                s_activeInstance = value;
            }
        }

        public static void RenderFrame(RenderTarget target) => s_activeInstance.Draw(target);

        #endregion

        public abstract void Draw(RenderTarget renderTarget);
    }
}
