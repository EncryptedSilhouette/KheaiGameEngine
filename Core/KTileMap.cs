﻿using SFML.Graphics;
using SFML.System;

namespace KheaiGameEngine.Core
{
    public class KTileMap
    {
        public ushort CellSizeX => Grid.CellSizeX;
        public ushort CellSizeY => Grid.CellSizeY;
        public ushort Rows => Grid.Rows;
        public ushort Columns => Grid.Columns;
        public Texture Texture { get; private set; }
        public KGrid<ushort> Grid { get; private set; }

        public KTileMap(KGrid<ushort> grid, Texture texture)
        {
            ushort count = 0;

            Texture = texture;
            Grid = grid;

            for (ushort i = 0; i < Rows; i++)
            {
                for (ushort j = 0; j < Columns; j++)
                {
                    grid[i, j] = count;
                    count++;
                }
            }
        }

        public KTileMap(ushort cellSizeX, ushort cellSizeY, ushort rows, ushort columns, Texture texture) :
            this(new KGrid<ushort>(cellSizeX, cellSizeY, rows, columns), texture) { }

        public Vector2f GetCoordinates(uint value)
        {
            uint x = value / Columns;
            uint y = value % Columns;
            return new Vector2f(x * CellSizeX, y * CellSizeY);
        }
    }
}
