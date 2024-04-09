using SFML;
using SFML.Graphics;

namespace KheaiGameEngine.Core
{

    public class KDrawHandler : KEngineComponent
    {
        protected RenderWindow window;
        protected KSceneHandler sceneHandler;

        public override void Init()
        {
           
        }

        public override void Start()
        {

        }

        public override void Update(uint currentTick)
        {

        }

        public override void FrameUpdate(uint currentFrame)
        {

        }

        public override void End()
        {

        }

        public void Draw(RenderTarget target)
        {
            
        }

        public void AddDrawComponent()
        {

        }

        public void AddReference()
        {

        }

        Texture GenerateTextureAtlas() 
        {
            Texture texture = null;
            return texture;
        }

        Texture GenerateTextureAtlas(string[] texturePaths) 
        {
            uint count = 0;
            uint largestSize = 0;

            uint rowHeight;

            Texture textureAtlas;
            List<(uint, uint)> sizes = new();   
            List<Image> images = new();
            
            foreach (var texturePath in texturePaths)
            {
                try
                {
                    Image image = new(texturePath);
                    images.Add(image);  
                }
                catch (LoadingFailedException)
                {

                    throw;
                }
            }

            images.Sort((a, b) =>
            {
                //The one with the greater Y should always be 1st
                if (a.Size.Y < b.Size.Y) return 1;
                else
                if (a.Size.Y > b.Size.Y) return -1;

                //If the Y is equal the one with the bigger X will be 1st
                if (a.Size.Y == b.Size.Y)
                {
                    if (a.Size.X < b.Size.X) return 1;
                    else
                    if (a.Size.X > b.Size.X) return -1;
                }

                //Only remaining case is they are equal (both X and Y)
                return 0;
            });

            for (int i = 0; i < images.Count; i++)
            {
                textureAtlas.Update();
            }   

            return textureAtlas;
        }
    }
}
