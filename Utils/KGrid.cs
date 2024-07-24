#if DEBUG

namespace KheaiGameEngine.GameManagement
{
    public class KGrid<T>
    {
        public T[,] Grid { get; private set; }
        public ushort CellSizeX { get; private set; }
        public ushort CellSizeY { get; private set; }
        public ushort Rows { get; private set; }
        public ushort Columns { get; private set; }
        public T this[ushort row, ushort collumn]
        {
            get => Grid[row, collumn];
            set => Grid[row, collumn] = value;
        }

        public KGrid(ushort cellSizeX, ushort cellSizeY, ushort rows, ushort columns)
        {
            Rows = rows;
            Columns = columns;
            CellSizeX = cellSizeX;
            CellSizeY = cellSizeY;
            Grid = new T[rows, columns];
        }
    }
}

#endif