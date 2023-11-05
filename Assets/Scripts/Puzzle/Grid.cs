namespace WordPuzzle
{
    public class Grid
    {
        public class Cell
        {
            public char val = '\0';
            public bool connectHoriz = false;
            public bool connectVert = false;
            public bool searched = false;
        }
        private int _width = 0;
        private int _height = 0;
        public Cell[] cells = null;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;
            cells = new Cell[width * height];
            for (int i = 0; i < cells.Length; ++i)
                cells[i] = new Cell();
        }

        public Cell GetCell(int x, int y)
        {
            return cells[y * _width + x];
        }

        public char[] GetCharArr()
        {
            char[] arr = new char[_width * _height];
            for (int i = 0; i < cells.Length; i++)
            {
                arr[i] = cells[i].val;
            }
            return arr;
        }
    }
}
