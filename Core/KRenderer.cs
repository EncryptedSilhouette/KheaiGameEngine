using SFML.Graphics;

namespace KheaiGameEngine
{
    public abstract class KRenderer
    {
        #region Static

        private static KRenderer s_activeInstance;

        ///<summary>Event handler for when the active instance is changed</summary>
        public static event Action OnInstanceChanged;

        ///<summary>Property for the static refrence to the active instance. Fires an event when the instance is changed.</summary>
        public static KRenderer ActiveInstance
        {
            get => s_activeInstance;
            set
            {
                OnInstanceChanged?.Invoke();
                s_activeInstance = value;
            }
        }

        ///<summary>Static method to render a frame using the active instance.</summary>
        public static void RenderFrame(RenderTarget target) => s_activeInstance.Draw(target);

        #endregion

        ///<summary>Abstract method to set the behavior for drawing.</summary>
        public abstract void Draw(RenderTarget renderTarget);
    }
}
