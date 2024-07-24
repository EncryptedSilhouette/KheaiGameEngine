#if DEBUG

using SFML.Graphics;
using SFML.System;

namespace KheaiGameEngine.GameManagement
{
    public class KTileMap : KGrid<uint>
    {
        public Texture Texture;

        public uint[,] Grid { get; set; }
        public uint CellWidth { get; set; }
        public uint CallHeight { get; set; }
        public uint Rows { get; set; }
        public uint Columns { get; set; }

        public KTileMap(uint cellSizeX, uint cellSizeY, uint rows, uint columns, Texture texture)
        {
            ulong  count = 0;

            Texture = texture;

            for (uint i = 0; i < Rows; i++)
            {
                for (uint j = 0; j < Columns; j++)
                {
                    Grid[i, j] = count;
                    count++;
                }
            }
        }

        //TODO: revise this
        public Vector2f GetTextureCoords(uint value)
        {
            uint x = value / Columns;
            uint y = value % Columns;
            return new Vector2f(x * CellWidth, y * CallHeight);
        }
    }
}

#endif
