using SFML.Graphics;

namespace KheaiGameEngine
{
    public abstract class KDrawHandler : KEngineComponent
    {
        private static KDrawHandler s_instance;

        public static event EventHandler OnInstanceChanged;

        public static KDrawHandler ActiveInstance 
        {
            get => s_instance;
            private set 
            {
                s_instance = value;
                OnInstanceChanged?.Invoke(value, null);
            } 
        }

        public abstract void Draw(RenderTarget target);
    }
}
