using KheaiGameEngine;
using SFML.Graphics;

namespace KheaiUtils
{
    public class KTextureAtlas 
    {
        private record class TextureData(string id, Image image);

        private Texture _atlas;
        private Dictionary<string, Vertex[]> _texCoords;
        private List<TextureData> _textureData; 

        public Texture Atlas => _atlas;

        public Vertex[] this[string textureID] => GetTexCoords(textureID);

        public Vertex[] GetTexCoords(string textureID) => _texCoords[textureID];

        public void StartAtlas() 
        {
            _texCoords = new();
            _textureData = new();
        }

        public void SubmitTexture(string filePath) 
        {
            string textureID = Path.GetFileNameWithoutExtension(filePath);
            //_images.Add(textureID, )
        }

        public void CreateAtlas() 
        {
            uint rowHeight;
            uint rowXOffset = 0;
            uint rowYOffset = 0;
            Stack<TextureData> baseImages = new(); 

            _textureData.Sort((a, b) => 
            {
                //Sort by height.
                if (a.image.Size.Y > b.image.Size.Y) return 1;
                if (a.image.Size.Y < b.image.Size.Y) return -1;

                //If height is the same sort by width.
                if (a.image.Size.X > b.image.Size.X) return 1;
                if (a.image.Size.X < b.image.Size.X) return -1;

                //return 0 if both width & height are the same.
                return 0;
            });

            Image image;
            rowHeight = _textureData[0].image.Size.Y;

            for (int i = 0; i < _textureData.Count; i++)
            {
                baseImages.Push(_textureData[i]);
                rowYOffset = _textureData[i].image.Size.Y; 

                for (int j = i + 1; j < rowHeight; j++)
                {
                    if (rowYOffset >= rowHeight) break;
                }


            }

            _textureData.Clear();
            _textureData.TrimExcess();
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
