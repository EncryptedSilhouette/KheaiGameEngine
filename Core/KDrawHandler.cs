using SFML.Graphics;
using SFML.System;
using System.Numerics;

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
            uint currentX = 0, currentY = 0;
            uint xOffset = 0, yOffset = 0;
            uint rowHeight, rowStart;

            Texture combinedTexture;
            List<Image> images = new();

            foreach (var path in texturePaths) images.Add(new(path));

            images.Sort((a, b) => 
            {
                if (a.Size.Y > b.Size.Y) return 1;
                else return -1;
            });

            rowHeight = images[0].Size.Y;

            for (int i = 0; i < images.Count; i++)
            {
                if (images[i].Size < ) 
                {
                }
            }

            return combinedTexture;
        }
    }
}
