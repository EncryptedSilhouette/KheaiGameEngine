#if DEBUG

namespace KheaiGameEngine.WIP
{
    public interface IKGrid<T>
    {
        ///<summary>The grid's contents.</summary>
        T[,] Contents { get; set; }
        ///<summary>The width of a cell.</summary>
        uint CellWidth { get; set; }
        ///<summary>The height of a cell.</summary>
        uint CellHeight { get; set; }
        ///<summary>The number of Rows for the grid.</summary>
        uint Rows { get; set; }
        ///<summary>The number of Collumns for the grid.</summary>
        uint Columns { get; set; }
    }
}

#endif