namespace Elements.Core
{
    public struct KGrid
    {
        public int CellWidth;
        public int CellHeight;

        public int Rows { get; private set; } 
        public int Columns { get; private set; }
        public int CellCount => Cells.Length;
        public int[] Cells {  get; private set; }

        public KGrid(int rows, int columns, int cellWidth, int cellHeight)
        {
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            Rows = rows;
            Columns = columns;
            Cells = new int[rows * columns];

            for (int i = 0; i < Cells.Length; i++) Cells[i] = 0;
        }

        public ref int GetCell(int row, int column)
        {
            return ref Cells[row * Columns + column];
        }
    }
}
