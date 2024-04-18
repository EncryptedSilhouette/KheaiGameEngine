using KheaiGameEngine.Debug;
using SFML.Graphics;
using SFML.System;

namespace KheaiGameEngine.Core
{

    public class KDrawHandler : KEngineComponent
    {
        protected Texture textureAtlas;
        protected RenderWindow window;
        protected KSceneHandler sceneHandler;
        protected Dictionary<string, Vector2f[]> textureCoords = new();

        protected Vector2f[] this[string textureID] => textureCoords[textureID];

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

        protected Texture GenerateTextureAtlas() 
        {
            Texture texture = null;
            return texture;
        }

        protected Texture GenerateTextureAtlas(string[] texturePaths) 
        {
            uint rowHeight = 0;
            uint yOffset = 0;
            uint xOffset = 0;

            Texture textureAtlas;
            List<(string id, Image image)> images = new();
            
            //Load textures in path
            foreach (var path in texturePaths)
            {
                try
                {
                    string textureID = path.Substring(0, path.IndexOf('.'));
                    Image image = new(path);
                    images.Add((textureID, image));  
                }
                catch (SFML.LoadingFailedException exception)
                {
                    KDebugger.ErrorLog($"Failed loading resource: {path}");
                    KDebugger.ErrorLog(exception.Message);
                }
            }

            //Sort textures by height and then by width
            images.Sort((a, b) =>
            {
                //The one with the greater height should always be 1st
                if (a.image.Size.Y < b.image.Size.Y) return 1;
                if (a.image.Size.Y > b.image.Size.Y) return -1;

                //If the height is equal 
                //The one with the greater width will be 1st
                if (a.image.Size.X < b.image.Size.X) return 1;
                if (a.image.Size.X > b.image.Size.X) return -1;

                //Only remaining case is both dimentions are equal to the other's
                return 0;
            });

            //Minimize empty space by stacking images where possible
            //Use the first image in the order to set the size for the row
            rowHeight = images[0].image.Size.Y;

            //Iterate over sorted images
            for (int i = 0; i < images.Count; i++)
            {
                //Define a base image to stack upon
                Image baseImage = images[i].image;

                textureCoords.Add(images[i].id, 
                new Vector2f[] 
                {
                    new(xOffset, 0),                  new(xOffset + baseImage.Size.X, 0),
                    new(xOffset, baseImage.Size.Y),   new(xOffset + baseImage.Size.X, baseImage.Size.Y)
                });
                 
                //Set the offsets
                yOffset = baseImage.Size.Y;      
                xOffset += images[i].image.Size.X;

                //If current image height is less than the row height, create a section
                if (baseImage.Size.Y < rowHeight) 
                {
                    //Go through images starting at the one after the base image
                    for (int j = i + 1; j < images.Count; j++)
                    {
                        Image imageJ = images[i];

                        //Check if image can stack and be within bounds 
                        //The height of the section cannot exceed the row height
                        //The length of the stacked image cannot exceed the base width
                        if (baseImage.Size.Y + imageJ.Size.Y <= rowHeight &&
                            baseImage.Size.X >= imageJ.Size.X) 
                        {
                            
                        }
                    } 
                }
            }

            return textureAtlas;
        }
    }
}
