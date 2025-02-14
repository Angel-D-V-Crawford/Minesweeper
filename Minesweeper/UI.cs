using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    internal class UI
    {

        private Pen pen;
        private Pen boardPen;
        private Brush brush;
        private Brush numberBrush;
        private Font font;
        private Font numberFont;

        public int BoxSize { get; }

        public int BoardRows;
        public int BoardCols;

        public int StartBoardX;
        public int StartBoardY;
        public int WidthLength;
        public int HeightLength;

        public int RemainingMines { get; set; }

        private static Image mineImage;
        private static Image flagImage;

        public UI(int boardRows, int boardCols, int numMines, int boxSize)
        {
            pen = new Pen(Color.Black);
            boardPen = new Pen(Color.White);
            boardPen.Width = 2;
            brush = new SolidBrush(Color.White);
            numberBrush = null;

            font = new Font(new FontFamily("Consolas"), 30);

            int fontSize = 30;
            if (boxSize == 40) fontSize = 25;
            numberFont = new Font(new FontFamily("Consolas"), fontSize);

            RemainingMines = numMines;
            BoardRows = boardRows;
            BoardCols = boardCols;

            BoxSize = boxSize;

            mineImage = Properties.Resources.mine;
            flagImage = Properties.Resources.flag;
        }

        public void render(Graphics g, Size size)
        {

            int halfX = size.Width / 2;
            int halfY = size.Height / 2;
            int halfBox = BoxSize / 2;
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            string text = string.Format("{0}", RemainingMines);

            if (MinesweeperGame.GameOver)
                text = "You lose :(";

            if (MinesweeperGame.GameWin)
                text = "You won! :)";

            g.Clear(Color.Black);
            g.DrawString( text, font, brush, halfX, 20, stringFormat );
            
            drawBoard( g, halfX, halfY );

        } // end render

        private void drawBoard(Graphics g, int centerX, int centerY)
        {

            WidthLength = BoxSize * BoardCols;
            HeightLength = BoxSize * BoardRows;

            int halfWidth = WidthLength / 2;
            int halfHeight = HeightLength / 2;

            StartBoardX = centerX - halfWidth;
            StartBoardY = centerY - halfHeight;
            int endX = centerX + halfWidth;
            int endY = centerY + halfHeight;

            int offsetX, offsetY;

            // Draw blocks
            for (int i = 0; i < BoardRows; i++)
            {
                offsetY = i * BoxSize;
                for (int j = 0; j < BoardCols; j++)
                {
                    offsetX = j * BoxSize;
                    drawBlock(g, i, j, StartBoardX + offsetX, StartBoardY + offsetY);
                }
            }

            // Draw borders
            g.DrawRectangle(boardPen, StartBoardX, StartBoardY, WidthLength, HeightLength);

        } // end drawBoard

        private void drawBlock(Graphics g, int row, int col, int left, int top)
        {
            int cell = MinesweeperGame.Board[row, col];
            int mine = MinesweeperGame.Mines[row, col];

            if(cell == MinesweeperGame.UNOPENED)
            {
                g.FillRectangle(brush, left + 1, top + 1, BoxSize - 2, BoxSize - 2);
            }
            if(cell == MinesweeperGame.FLAGGED)
            {
                g.DrawImage(flagImage, left + 1, top + 1, BoxSize - 2, BoxSize - 2);
            }
            if(cell == MinesweeperGame.OPENED && mine != MinesweeperGame.MINE)
            {
                drawNumber(g, left, top, mine);
            }
            if (cell == MinesweeperGame.OPENED && mine == MinesweeperGame.MINE)
            {
                g.DrawImage(mineImage, left + 1, top + 1, BoxSize - 2, BoxSize - 2);
            }
        }

        private void drawNumber(Graphics g, int left, int top, int number)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            int numberX = left + BoxSize / 2;
            //Debug.WriteLine(number);

            switch (number)
            {
                case 0: 
                    g.DrawRectangle(pen, left, top, BoxSize, BoxSize); 
                    break;

                case 1:
                    numberBrush = new SolidBrush(Color.Blue);
                    g.DrawString(string.Format("{0}", number), numberFont, numberBrush, numberX, top, stringFormat);
                    break;

                case 2:
                    numberBrush = new SolidBrush(Color.Green);
                    g.DrawString(string.Format("{0}", number), numberFont, numberBrush, numberX, top, stringFormat);
                    break;

                case 3:
                    numberBrush = new SolidBrush(Color.Red);
                    g.DrawString(string.Format("{0}", number), numberFont, numberBrush, numberX, top, stringFormat);
                    break;

                case 4:
                    numberBrush = new SolidBrush(Color.Purple);
                    g.DrawString(string.Format("{0}", number), numberFont, numberBrush, numberX, top, stringFormat);
                    break;

                case 5:
                    numberBrush = new SolidBrush(Color.Brown);
                    g.DrawString(string.Format("{0}", number), numberFont, numberBrush, numberX, top, stringFormat);
                    break;

                case 6:
                    numberBrush = new SolidBrush(Color.Turquoise);
                    g.DrawString(string.Format("{0}", number), numberFont, numberBrush, numberX, top, stringFormat);
                    break;

                case 7:
                    numberBrush = new SolidBrush(Color.White);
                    g.DrawString(string.Format("{0}", number), numberFont, numberBrush, numberX, top, stringFormat);
                    break;

                case 8:
                    numberBrush = new SolidBrush(Color.Gray);
                    g.DrawString(string.Format("{0}", number), numberFont, numberBrush, numberX, top, stringFormat);
                    break;
            }
        } // end drawNumber

        public void openCell(MouseEventArgs e, bool isFirstClick)
        {
            Cell cell = searchCell(e.Location);
            if(MinesweeperGame.Board[cell.Row, cell.Col] != MinesweeperGame.FLAGGED 
                && MinesweeperGame.Board[cell.Row, cell.Col] != MinesweeperGame.OPENED)
            {
                if (MinesweeperGame.Mines[cell.Row, cell.Col] == MinesweeperGame.MINE)
                {
                    if (isFirstClick)
                    {
                        MinesweeperGame.changeMine(cell.Row, cell.Col);
                        Debug.WriteLine(isFirstClick);
                        Debug.WriteLine(string.Format("{0}", MinesweeperGame.Mines[cell.Row, cell.Col]));
                    }
                    else
                    {
                        MinesweeperGame.GameOver = true;
                    }
                    
                }   

                MinesweeperGame.Board[cell.Row, cell.Col] = MinesweeperGame.OPENED;
                MinesweeperGame.openCellsCount++;
                MinesweeperGame.floodFill(cell.Row, cell.Col);
            }
        }

        private void printBoard()
        {
            for (int i = 0; i < MinesweeperGame.Board.GetLength(0); i++)
            {
                for (int j = 0; j < MinesweeperGame.Board.GetLength(1); j++)
                {
                    Debug.Write(string.Format("{0} ", MinesweeperGame.Board[i, j]));
                }
                Debug.WriteLine("");
            }
            Debug.WriteLine("");
        }

        public void flagCell(MouseEventArgs e)
        {
            Cell cell = searchCell(e.Location);
            int boardCell = MinesweeperGame.Board[cell.Row, cell.Col];

            if (boardCell == MinesweeperGame.UNOPENED)
            {
                MinesweeperGame.Board[cell.Row, cell.Col] = MinesweeperGame.FLAGGED;
                RemainingMines--;
                MinesweeperGame.NumMines = RemainingMines;
                MinesweeperGame.flaggedCellsCount++;

                MinesweeperGame.checkWin();
            }
            if(boardCell == MinesweeperGame.FLAGGED)
            {
                MinesweeperGame.Board[cell.Row, cell.Col] = MinesweeperGame.UNOPENED;
                RemainingMines++;
                MinesweeperGame.NumMines = RemainingMines;
                MinesweeperGame.flaggedCellsCount--;
            }
        }

        public Cell searchCell(Point point)
        {
            Rectangle rect = new Rectangle();
            rect.Width = BoxSize;
            rect.Height = BoxSize;

            int offsetX, offsetY;
            int row = 0;
            int col = 0;

            bool found = false;

            for (int i = 0; i < BoardRows && !found; i++)
            {
                offsetY = i * BoxSize;
                for (int j = 0; j < BoardCols && !found; j++)
                {
                    offsetX = j * BoxSize;
                    rect.X = StartBoardX + offsetX;
                    rect.Y = StartBoardY + offsetY;

                    if (rect.Contains(point)) found = true;
                    col = j;
                }
                row = i;
            }

            Cell cell = new Cell(rect, row, col);
            return cell;
        }

    }
}