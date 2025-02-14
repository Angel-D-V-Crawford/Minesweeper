using System.Drawing;

namespace Minesweeper
{
    internal class Cell
    {

        public Rectangle Rect { get; set; }

        public int Row { get; set; }
        public int Col { get; set; }

        public Cell(Rectangle rect, int x, int y)
        {
            this.Rect = rect;
            this.Row = x;
            this.Col = y;
        }

    }
}