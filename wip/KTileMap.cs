#if DEBUG

using SFML.Graphics;
using SFML.System;

namespace KheaiGameEngine.WIP
{
    public class KTileMap : IKGrid<Vertex[]>
    {
        ///<summary>The texture for the tilemap.</summary>
        public Texture Texture;

        public Vertex[,][] Contents { get; set; }
        public uint CellWidth { get; set; }
        public uint CellHeight { get; set; }
        public uint Rows { get; set; }
        public uint Columns { get; set; }

        ///<summary>Gets the number of indices.</summary>
        public ulong Count => Rows * Columns;
        ///<summary>Gets the texture coordinates at the specided index.</summary>
        Vertex[] this[uint index] => GetTextureCoords(index);
        ///<summary>Gets the texture coordinates at the specided row and collumn.</summary>
        Vertex[] this[uint row, uint collumn] => Contents[row, collumn];

        public KTileMap(uint cellSizeX, uint cellSizeY, uint rows, uint columns, Texture texture)
        {
            ulong count = 0;

            Texture = texture;

            for (uint i = 0; i < Rows; i++)
            {
                for (uint j = 0; j < Columns; j++)
                {
                    Contents[i, j] = new Vertex[]
                    {
                        new Vertex(new Vector2f(), new Vector2f(j * CellWidth, i * CellHeight)),
                        new Vertex(new Vector2f(), new Vector2f((j + 1) * CellWidth, i * CellHeight)),
                        new Vertex(new Vector2f(), new Vector2f((j + 1) * CellWidth, (i + 1) * CellHeight)),
                        new Vertex(new Vector2f(), new Vector2f(j * CellWidth, (i + 1) * CellHeight)),
                    };
                    count++;
                }
            }
        }

        ///<summary>Gets the texture coordiantes at index.</summary>
        public Vertex[] GetTextureCoords(uint index)
        {
            uint row = index / Columns;
            uint collumn = index % Columns;
            return this[row, collumn];
        }

        ///<summary>Gets the texture at index.</summary>
        public Texture GetTexture(uint index)
        {
            uint row = index / Columns;
            uint collumn = index % Columns;
            RenderTexture texture = new(CellWidth, CellHeight);
            VertexArray verticies = new();

            verticies.Append(this[row, collumn][0]);
            verticies.Append(this[row, collumn][1]);
            verticies.Append(this[row, collumn][2]);
            verticies.Append(this[row, collumn][3]);
            texture.Draw(verticies);

            return texture.Texture;
        }
    }
}

#endif