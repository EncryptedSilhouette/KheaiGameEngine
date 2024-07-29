using SFML.Graphics;

namespace KheaiGameEngine
{
    public interface IKRenderer
    {
        ///<summary>Defines the behavior for drawing.</summary>
        ///<param name = "target">The render target to draw to.</param>
        public void Render(RenderTarget target);
    }
}
