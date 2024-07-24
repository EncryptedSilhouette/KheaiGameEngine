namespace KheaiGameEngine.GameManagement
{
    public interface KGrid<T>
    {
        T[,] Grid { get; set; }
        uint CellWidth { get; set; }
        uint CallHeight { get; set; }
        uint Rows { get; set; }
        uint Columns { get; set; }
        T this[uint row, uint collumn]
        {
            get => Grid[row, collumn];
            set => Grid[row, collumn] = value;
        }
    }
}