namespace Elements.Core
{
    public struct KGrid
    {
        public int CellWidth;
        public int CellHeight;

        public int CellCount => Cells.Length;
        public int Rows { get; private set; } 
        public int Columns { get; private set; }
        public int[] Cells { get; private set; }

        public int this[int index] 
        {
            get => Cells[index]; 
            set => Cells[index] = value;
        }

        public int this[int row, int column]
        {
            get
            {
                if (0 <= column && column < Columns && 0 <= row && row < Rows)
                {
                    return Cells[row * Columns + column];
                }
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (0 <= column && column < Columns && 0 <= row && row < Rows)
                {
                    Cells[row * Columns + column] = value;
                }
                else throw new IndexOutOfRangeException();
            }
        }

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
