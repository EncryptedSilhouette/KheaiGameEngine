using KheaiGameEngine.Debug;
using SFML.Graphics;
using SFML.System;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KheaiGameEngine.Core
{

    public class KDrawHandler : KEngineComponent
    {
        protected Sprite sprite;
        protected Texture textureAtlas;
        protected RenderStates renderStates;
        protected RenderWindow window;
        protected KSceneHandler sceneHandler;
        protected Dictionary<string, Vector2f[]> textureLookup = new();

        protected Vector2f[] this[string textureID] => textureLookup[textureID];

        public override void Init()
        {
            Test();
            sprite = new(textureAtlas);
            sprite.Position = new(64, 64);
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
            if (textureAtlas == null) Console.WriteLine("Hey dumbfuck somethings wrong");
            target.Draw(sprite);
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

        public void Test() 
        {
            textureAtlas = GenerateTextureAtlas(
                JsonSerializer.Deserialize<List<string>>(File.ReadAllText("test.json")).ToArray());
        }

        protected Texture GenerateTextureAtlas(string[] texturePaths) 
        {
            bool canFitAnother;
            uint rowLength;
            uint rowHeight;
            uint sectionWidth;
            uint sectionHeight;
            uint sectionXOffset;
            uint sectionYOffset;
            Image sectionBaseImage;
            Image sectionImage;

            Texture textureAtlas;
            List<(string id, Image image)> images = new();
            
            //Load textures in path
            foreach (var path in texturePaths)
            {
                try
                {
                    string textureID = path.Substring(0, path.IndexOf('.'));
                    images.Add((textureID, new("res\\debug\\" + path)));
                    Console.WriteLine($"Loaded Texture: {textureID}");
                }
                catch (SFML.LoadingFailedException exception)
                {
                    Console.WriteLine(exception.Message);
                    /*KDebugger.ErrorLog($"Failed loading resource: {path}");
                    KDebugger.ErrorLog(exception.Message);*/
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

            rowLength = 0;
            rowHeight = images[0].image.Size.Y;

            Console.WriteLine();

            //Iterate over sorted all images
            for (int indexI = 0; indexI < images.Count; indexI++)
            {
                Console.WriteLine("On base tex: " + images[indexI].id);

                //Sets a base image for the section
                sectionBaseImage = images[indexI].image;

                Console.WriteLine("added base tex: " + images[indexI].id);

                //Adds the texture coords to the lookup 
                textureLookup.Add(images[indexI].id, new Vector2f[]
                {
                    new(rowLength, 0),                
                    new(rowLength + sectionBaseImage.Size.X, 0),
                    new(rowLength + sectionBaseImage.Size.X, sectionBaseImage.Size.Y),    
                    new(rowLength, sectionBaseImage.Size.Y)
                });

                //Set bounds
                rowLength += sectionBaseImage.Size.X;

                sectionXOffset = 0;
                sectionYOffset = sectionBaseImage.Size.Y;
                sectionWidth = sectionBaseImage.Size.X;
                sectionHeight = sectionBaseImage.Size.Y;

                canFitAnother = false;

                //Checks the next images ahead to try to fill in the section.
                for (int indexJ = indexI + 1; indexJ < images.Count; indexJ++)
                {
                    Console.WriteLine("On tex: " + images[indexJ].id);
                    sectionImage = images[indexJ].image;

                    //Given that the list is sorted by height and then length, this has been simplified with that in mind.
                    //The "section" that is mentioned is the empty space between the base image and the height of the row.
                    //The height of the row is the height of the first image.
                    //The following checks will check if the current image will fit into that section.
                    //If an image is too tall it is skipped;
                    //If an image is too long, check if it can fit in another row, if it can't it is skipped,
                    //if it can "canFitAnother" is set to true, and the loop is reset.

                    //If the image is too tall then move onto the next
                    if (sectionYOffset + sectionImage.Size.Y > rowHeight)
                    {
                        Console.WriteLine("Too tall");
                        continue;
                    }

                    //If the image fits vertically check if it horizontally fits into the section
                    if (sectionXOffset + sectionImage.Size.X > sectionWidth) 
                    {
                        Console.WriteLine("Too long");

                        //Check if the image can fit in a higher row
                        if (sectionImage.Size.X <= sectionWidth && 
                            sectionImage.Size.Y + sectionHeight <= rowHeight) 
                            canFitAnother = true;

                        //If the last image in the list doesnt fit, move to the next row
                        if (indexJ == images.Count - 1 && canFitAnother) 
                        {
                            //reset the loop with new bounds for the next row of the 
                            canFitAnother = false;
                            sectionXOffset = 0;
                            sectionYOffset = sectionHeight;
                            indexJ = indexI;
                        }
                        continue;
                    }

                    Console.WriteLine("added tex: " + images[indexJ].id);

                    //Add the texure coords to the lookup
                    textureLookup.Add(images[indexJ].id, new Vector2f[]
                    {
                        new(rowLength + sectionXOffset, sectionYOffset),
                        new(rowLength + sectionXOffset + sectionBaseImage.Size.X, sectionYOffset),
                        new(rowLength + sectionXOffset + sectionBaseImage.Size.X, sectionYOffset + sectionBaseImage.Size.Y),  
                        new(rowLength + sectionXOffset, sectionYOffset + sectionBaseImage.Size.Y)
                    });

                    if (sectionXOffset == 0) sectionHeight = sectionYOffset + sectionImage.Size.Y;

                    //Increment sectionXOffset to include image length
                    sectionXOffset += sectionImage.Size.X;

                    //This shouldnt cause any issues since this loop is an index ahead of the other one.
                    images.RemoveAt(indexJ);

                    //Reset the loop and remove added texture from the list so that it is not iterated over in the future.
                    indexJ = indexI;
                }
            }

            //Create textureAtlas 
            textureAtlas = new(rowLength, rowHeight);

            //Add images to textureAtlas
            foreach (var image in images) 
            {
                Vector2f coords = textureLookup[image.id][0];
                textureAtlas.Update(image.image, (uint) coords.X, (uint) coords.Y);
            }

            Console.WriteLine("Finished Atlas");

            return textureAtlas;
        }
    }
}
