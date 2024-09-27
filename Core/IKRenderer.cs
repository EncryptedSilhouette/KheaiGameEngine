using SFML.Graphics;

namespace KheaiGameEngine.Core
{
    public interface IKRenderer
    {
        ///<summary>Defines the behavior for drawing stuff to the screen.</summary>
        ///<param name = "target">The SFML RenderTarget to draw to.</param>
        public void Render(RenderTarget target);
    }
}
