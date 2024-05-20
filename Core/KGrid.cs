namespace KheaiGameEngine.Core
{
    public class KGrid<T>
    {
        public T[,] Grid { get; private set; }
        public ushort CellSizeX { get; private set; }
        public ushort CellSizeY { get; private set; }
        public ushort Rows { get; private set; }
        public ushort Columns { get; private set; }
        public T this[ushort x, ushort y] => Grid[x, y];

        public KGrid(ushort cellSizeX, ushort cellSizeY, ushort rows, ushort columns)
        {
            CellSizeX = cellSizeX;
            CellSizeY = cellSizeY;
            Rows = rows;
            Columns = columns;
            Grid = new T[rows, columns];
        }
    }
}
