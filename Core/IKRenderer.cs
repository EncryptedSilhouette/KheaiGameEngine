namespace KheaiGameEngine.Core
{
    public interface IKRenderer : IKEngineObject
    {
        ///<summary>Defines the behavior for drawing stuff to the screen.</summary>
        ///<param name = "target">The SFML RenderTarget to draw to.</param>
        public void RenderFrame(ulong currentFrame);
    }
}
