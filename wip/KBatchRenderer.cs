#if DEBUG

using SFML.Graphics;
using SFML.System;

namespace KheaiGameEngine.wip
{
    public class KTextureAtlas
    {
        //Wrapper to associate an id with a texture
        private record class TextureData(string id, Texture texture);

        //Atlasing variables
        private Texture _atlas;
        private Dictionary<string, Vector2f[]> _textureCoords = new();
        private List<TextureData> _textureData = new();

        ///<summary>Gets the current texture atlas.</summary>
        public Texture Atlas => _atlas;

        ///<summary>Indexer to get the texture coords of a given texture.</summary>
        public Vector2f[] this[string textureID] => GetTexCoords(textureID);

        ///<summary>Gets the texture coords of a given texture.</summary>
        public Vector2f[] GetTexCoords(string textureID)
        {
            if (_textureCoords is null || _textureCoords.Count < 1)
            {
                //KDebugger.Log(KDebugger.ERROR_LOG, "No textures have been submitted to the atlas.");
                return null;
            }
            if (_textureCoords.ContainsKey(textureID)) return _textureCoords[textureID];

            //KDebugger.Log(KDebugger.ERROR_LOG, $"Texture does not exist: {textureID}.");
            return default;
        }

        ///<summary>Submit a texture with an associated id to be atlased.</summary>
        public void SubmitTexture(string id, Texture texture)
        {
            if (_textureData is null) _textureData = new();

            _textureData.Add(new TextureData(id, texture));
        }

        ///<summary>Load and submit a texture; Associates an id based on the filepath. 
        ///Returns true if the texture was sucessfully loaded.</summary>
        public bool LoadTexture(string filePath)
        {
            try
            {
                SubmitTexture(Path.GetFileNameWithoutExtension(filePath), new(filePath));
                return true;
            }
            catch (SFML.LoadingFailedException e)
            {
                //KDebugger.ErrorLog(e.Message);
                return false;
            }
        }

        ///<summary>Creates a texture atlas using the submitted textures.</summary>
        public Texture CreateAtlas()
        {
            if (_textureData is null || _textureData.Count < 1)
            {
                //KDebugger.Log(KDebugger.ERROR_LOG, "No textures have been submitted to the atlas.");
                return null;
            }

            uint rowLength, rowHeight;

            _textureCoords = new();
            _textureData.Sort((a, b) =>
            {
                //Sort textures by height and then by width
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

                //Skip any texture already added
                if (_textureCoords.ContainsKey(_textureData[i].id)) continue;

                //Adds the texture coords to the lookup 
                _textureCoords.Add(_textureData[i].id, new Vector2f[]
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

                //Checks the next texture ahead to try to fill in the section.
                for (int j = i + 1; j < _textureData.Count; j++)
                {
                    //Skip any image already added
                    if (_textureCoords.ContainsKey(_textureData[j].id)) continue;

                    //Given that the list is sorted by height and then length, this has been simplified with that in mind.
                    //That is to say the next texture will always be the same height or shorter.
                    //The "section" that is mentioned is the empty space between the base texture and the height of the row.
                    //The height of the row is the height of the first image.
                    //The following checks will check if the current texture will fit into that section.
                    //If an texture is too tall it is skipped;
                    //If an texture is too long, check if it can fit in another row.
                    //If an texture can fit in a row, then on the last index, reset the counter and increase the Y offset

                    //If the texture is too tall then move onto the next
                    if (sectionYOffset + _textureData[j].texture.Size.Y > rowHeight) continue;

                    //If the texture fits vertically check if it horizontally fits into the section
                    if (sectionXOffset + _textureData[j].texture.Size.X > sectionLength)
                    {
                        //Check if the texture can fit in a higher row
                        if (_textureData[j].texture.Size.X <= sectionLength &&
                            _textureData[j].texture.Size.Y + sectionHeight <= rowHeight)
                            canFitAnother = true;

                        //If an texture can fit in a higher row, on the last index reset the counter,
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
                    _textureCoords.Add(_textureData[j].id, new Vector2f[]
                    {
                        new(sectionXOrigin + sectionXOffset, sectionYOffset),
                        new(sectionXOrigin + sectionXOffset + _textureData[j].texture.Size.X, sectionYOffset),
                        new(sectionXOrigin + sectionXOffset + _textureData[j].texture.Size.X, sectionYOffset + _textureData[j].texture.Size.Y),
                        new(sectionXOrigin + sectionXOffset, sectionYOffset + _textureData[j].texture.Size.Y)
                    });

                    //If starting a new row (sectionXOffset == 0),
                    //set the section height to the offset plus the height of first texture in the row
                    if (sectionXOffset == 0) sectionHeight = sectionYOffset + _textureData[j].texture.Size.Y;

                    //Increment the offset by the texture length
                    sectionXOffset += _textureData[j].texture.Size.X;

                    //Reset the iterator counter 
                    j = i;
                }
            }

            //Create a textureAtlas with given bounds
            _atlas = new(rowLength, rowHeight);

            //Append each texture to the texture atlas
            foreach (var textureData in _textureData)
            {
                Vector2f coordinates = _textureCoords[textureData.id][0];
                _atlas.Update(textureData.texture, (uint)coordinates.X, (uint)coordinates.Y);
            }
            _textureData = null; //Free the refrence for garbage collection.

            return _atlas;
        }
    }

    ///<summary>A Renderer that batches multiple draw calls into one.</summary>
    public class KBatchRenderer //: IKEngineComponent, IKRenderer
    {
        public static readonly int BACKGROUND = 0; //Static ID for background layer.

        private int _frameVertexCount = 0; //Keeps track of how many verticies are being drawn for the current frame.
        private VertexBuffer _vertexBuffer; //A buffer to batch multiple vertexes.
        private List<List<Vertex>> _drawLayer = new(8); //A List to keep track of verticies for each layer.

        //TODO: Contains a better way to go about this
        public KTextureAtlas TextureAtlas;
        public RenderStates RenderStates;

        public short Order { get; init; }
        public string ID { get; init; }
        //public KEngine Engine { get; set; }

        public KBatchRenderer(uint vertexCount, in RenderStates renderStates, PrimitiveType primitiveType = PrimitiveType.Quads) : base()
        {
            RenderStates = renderStates;
            _vertexBuffer = new(vertexCount, primitiveType, VertexBuffer.UsageSpecifier.Stream);
        }

        public void Init() { }
        public void Start() { }
        public void End() { }

        public void Update(uint currentUpdate) { }
        public void FrameUpdate(uint currentUpdate) { }

        ///<summary>Draws batch to render target.</summary>
        public void Render(RenderTarget target)
        {
            //offset to keep track of array position
            int offset = 0;
            //a vertex array to update the vertexbuffer
            Vertex[] vertices = new Vertex[_frameVertexCount];

            //copies the verticies from each layer to the vertex array
            foreach (var layer in _drawLayer)
            {
                layer.CopyTo(vertices, offset);
                offset += vertices.Length;
            }

            //Update vertexbuffer and draw verticies using Renderstates. 
            _vertexBuffer.Update(vertices);
            _vertexBuffer.Draw(target, 0, (uint)_frameVertexCount, RenderStates);

            //Clear each layer.
            foreach (var layer in _drawLayer) layer.Clear();
        }

        ///<summary>Submit a draw call to be batched.</summary>
        public void Draw(int layer, in Vertex[] vertices)
        {
            //Adds new vertex layer if desired layer doesnt exist.
            if (_drawLayer.Capacity < layer + 1) _drawLayer.Capacity = layer + 1;
            //Adds verticies to layer.
            _drawLayer[layer].AddRange(vertices);
            //Increment vertex count by array length.
            _frameVertexCount += vertices.Length;
        }
    }
}
#endif