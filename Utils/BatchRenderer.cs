using KheaiGameEngine;
using KheaiGameEngine.Debug;
using SFML.Graphics;
using SFML.System;

namespace KheaiUtils
{
    public class KTextureAtlas 
    {
        private record class TextureData(string id, Texture texture);

        private Texture _atlas;
        private Dictionary<string, Vector2f[]> textureCoords;
        private List<TextureData> _textureData; 

        public Texture Atlas => _atlas;

        public Vector2f[] this[string textureID] => GetTexCoords(textureID);

        public Vector2f[] GetTexCoords(string textureID) => textureCoords[textureID];

        public void StartAtlas() 
        {
            textureCoords = new();
            _textureData = new();
        }

        public void SubmitTexture(string filePath) 
        {
            try
            {
                _textureData.Add(new TextureData(Path.GetFileNameWithoutExtension(filePath), new(filePath)));
            }
            catch (SFML.LoadingFailedException e)
            {
                KDebugger.ErrorLog(e.Message);
            }
        }

        public Texture CreateAtlas() 
        {
            uint rowLength, rowHeight;
            Texture atlas;

            //Sort textures by height and then by width
            _textureData.Sort((a, b) =>
            {
                //The one with the greater height should always be 1st
                if (a.texture.Size.Y < b.texture.Size.Y) return 1;
                if (a.texture.Size.Y > b.texture.Size.Y) return -1;

                //If the height is equal 
                //The one with the greater width will be 1st
                if (a.texture.Size.X < b.texture.Size.X) return 1;
                if (a.texture.Size.X > b.texture.Size.X) return -1;

                //Only remaining case is both dimentions are equal to the other's
                return 0;
            });

            rowLength = 0;
            rowHeight = _textureData[0].texture.Size.Y;

            //Iterate over sorted all images
            for (int i = 0; i < _textureData.Count; i++)
            {
                uint sectionXOffset, sectionYOffset, sectionXOrigin;
                uint sectionLength, sectionHeight;
                bool canFitAnother = false;

                //Skip any image already added
                if (textureCoords.ContainsKey(_textureData[i].id)) continue;

                //Adds the texture coords to the lookup 
                textureCoords.Add(_textureData[i].id, new Vector2f[]
                {
                    new(rowLength, 0),
                    new(rowLength + _textureData[i].texture.Size.X, 0),
                    new(rowLength + _textureData[i].texture.Size.X, _textureData[i].texture.Size.Y),
                    new(rowLength, _textureData[i].texture.Size.Y)
                });

                //Set bounds
                sectionXOrigin = rowLength;
                sectionXOffset = 0;
                sectionYOffset = _textureData[i].texture.Size.Y;
                sectionLength = _textureData[i].texture.Size.X;
                sectionHeight = _textureData[i].texture.Size.Y;
                rowLength += _textureData[i].texture.Size.X;

                //Checks the next images ahead to try to fill in the section.
                for (int j = i + 1; j < _textureData.Count; j++)
                {
                    //Skip any image already added
                    if (textureCoords.ContainsKey(_textureData[j].id)) continue;

                    //Given that the list is sorted by height and then length, this has been simplified with that in mind.
                    //That is to say the next image will always be the same height or shorter.
                    //The "section" that is mentioned is the empty space between the base image and the height of the row.
                    //The height of the row is the height of the first image.
                    //The following checks will check if the current image will fit into that section.
                    //If an image is too tall it is skipped;
                    //If an image is too long, check if it can fit in another row.
                    //If an image can fit in a row, then on the last index, reset the counter and increase the Y offset

                    //If the image is too tall then move onto the next
                    if (sectionYOffset + _textureData[j].texture.Size.Y > rowHeight) continue;

                    //If the image fits vertically check if it horizontally fits into the section
                    if (sectionXOffset + _textureData[j].texture.Size.X > sectionLength)
                    {
                        //Check if the image can fit in a higher row
                        if (_textureData[j].texture.Size.X <= sectionLength &&
                            _textureData[j].texture.Size.Y + sectionHeight <= rowHeight)
                            canFitAnother = true;

                        //If an image can fit in a higher row, on the last index reset the counter,
                        //and set the bounds for the next row
                        if (j == _textureData.Count - 1 && canFitAnother)
                        {
                            j = i - 1;
                            sectionXOffset = 0;
                            sectionYOffset = sectionHeight;
                            canFitAnother = false;
                        }
                        continue;
                    }

                    //Add the texure coords to the lookup
                    textureCoords.Add(_textureData[j].id, new Vector2f[]
                    {
                        new(sectionXOrigin + sectionXOffset, sectionYOffset),
                        new(sectionXOrigin + sectionXOffset + _textureData[j].texture.Size.X, sectionYOffset),
                        new(sectionXOrigin + sectionXOffset + _textureData[j].texture.Size.X, sectionYOffset + _textureData[j].texture.Size.Y),
                        new(sectionXOrigin + sectionXOffset, sectionYOffset + _textureData[j].texture.Size.Y)
                    });

                    //If starting a new row (sectionXOffset == 0),
                    //set the section height to the offset plus the height of first image in the row
                    if (sectionXOffset == 0) sectionHeight = sectionYOffset + _textureData[j].texture.Size.Y;

                    //Increment the offset by the image length
                    sectionXOffset += _textureData[j].texture.Size.X;

                    //Reset the iterator counter 
                    j = i;
                }
            }

            //Create a textureAtlas with given bounds
            atlas = new(rowLength, rowHeight);

            //Append each image to the texture
            foreach (var textureData in _textureData)
            {
                Vector2f coordinates = textureCoords[textureData.id][0];
                atlas.Update(textureData.texture, (uint)coordinates.X, (uint)coordinates.Y);
            }

            return atlas;
        }
    }

    public class BatchRenderer : KEngineComponent, IKDrawHandler
    {
        public static readonly int BACKGROUND = 0;

        private int _cycleVertexCount = 0;
        private VertexBuffer _vertexBuffer;
        private List<List<Vertex>> _drawCalls = new(8);

        public RenderStates RenderStates;

        public BatchRenderer(uint vertexCount, in RenderStates renderStates, PrimitiveType primitiveType = PrimitiveType.Quads) : base() 
        {
            RenderStates = renderStates;
            _vertexBuffer = new(vertexCount, primitiveType, VertexBuffer.UsageSpecifier.Stream);
        }
        
        public override void Init() { } 
        public override void Start() { }
        public override void End() { }
        public override void Update(uint currentUpdate) { }
        public override void FrameUpdate(uint currentUpdate) { }

        public void Render(RenderTarget target)
        {
            int offset = 0;
            Vertex[] vertices = new Vertex[_cycleVertexCount];

            foreach (var layer in _drawCalls) 
            {
                layer.CopyTo(vertices, offset);
                offset += vertices.Length;
            }

            _vertexBuffer.Update(vertices);
            _vertexBuffer.Draw(target, 0, (uint) _cycleVertexCount, RenderStates);

            foreach (var layer in _drawCalls) layer.Clear();
        }

        public void Draw(int layer, in Vertex[] vertices) 
        {
            if (_drawCalls.Capacity < layer + 1) _drawCalls.Capacity = layer + 1;
            _drawCalls[layer].AddRange(vertices);
            _cycleVertexCount += vertices.Length;
        }
    }
}
