using KheaiGameEngine;
using SFML.Graphics;
using SFML.System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace KheaiUtils
{
    public class KTextureAtlas 
    {
        private Texture _atlas;
        private Dictionary<string, Vector2f> _texCoords = new();

        public List<(string id, Image image)> images = new(); 

        private Texture Atlas => _atlas;

        public Vector2f this[string textureID] => GetTexCoords(textureID);

        public Vector2f GetTexCoords(string textureID) => _texCoords[textureID];

        public void SubmitTexture(string filePath) 
        {
            string textureID = Path.GetFileNameWithoutExtension(filePath);

        }

        public void CreateAtlas() 
        {

        }
    }

    public class BatchRenderer : KEngineComponent, IKDrawHandler
    {
        private int _cycleVertexCount = 0;
        private VertexBuffer _vertexBuffer;
        private List<(int layer, Vertex[] vertices)> _drawCalls = new();

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

            _drawCalls.Sort((a, b) => 
            {
                if (a.layer > b.layer) return 1;
                if (a.layer < b.layer) return -1;
                return 0;
            });

            foreach (var vertexArray in _drawCalls)
            {
                vertexArray.vertices.CopyTo(vertices, offset);
                offset += vertexArray.vertices.Length;
            }

            _vertexBuffer.Update(vertices);
            _vertexBuffer.Draw(target, 0, (uint) _cycleVertexCount, RenderStates);
            _drawCalls.Clear();
        }

        public void Draw(int layer, in Vertex[] vertices) 
        {
            _drawCalls.Add((layer, vertices));
            _cycleVertexCount += vertices.Length;
        }
    }
}
