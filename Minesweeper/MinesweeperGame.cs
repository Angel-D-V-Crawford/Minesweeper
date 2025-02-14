using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Minesweeper
{
    internal class MinesweeperGame
    {

        public static bool Running { get; set; }

        public static int BoardRows { get; set; }
        public static int BoardCols { get; set; }
        public static int NumMines { get; set; }
        private static int initialMines;
        public static int openCellsCount;
        public static int flaggedCellsCount;

        private bool isFirstClick;
        private bool clickToNewGame;
        public static bool GameOver { get; set; }
        public static bool GameWin { get; set; }

        public UI Ui;

        public const int MINE = -1;
        public const int UNOPENED = 0;
        public const int OPENED = 1;
        public const int FLAGGED = 2;

        public static int[,] Mines { get; set; }
        public static int[,] Board { get; set; }

        private static PictureBox Screen;

        public Thread GameLoopThread;

        public MinesweeperGame(int boardRows, int boardCols, int numMines, int boxSize, PictureBox screen)
        {
            BoardRows = boardRows;
            BoardCols = boardCols;
            NumMines = numMines;
            initialMines = numMines;
            openCellsCount = 0;
            flaggedCellsCount = 0;

            isFirstClick = true;
            clickToNewGame = false;

            GameOver = false;
            GameWin = false;

            Ui = new UI(boardRows, boardCols, numMines, boxSize);

            Mines = new int[boardRows, boardCols];
            Board = new int[boardRows, boardCols];

            Screen = screen;

            generateMines();
            initNeighbors();

            Running = true;

            printBoard();

            GameLoopThread = new Thread(gameLoop);
            GameLoopThread.Start();
        }

        private void printBoard()
        {
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    Debug.Write(string.Format("{0} ", Mines[i, j]));
                }
                Debug.WriteLine("");
            }
            Debug.WriteLine("");
        }

        private void generateMines()
        {
            for(int i = 0; i < NumMines; i++)
            {
                generateMine();
            }
        }

        private static void generateMine()
        {
            Random r = new Random();
            int row;
            int col;

            do
            {
                row = r.Next(0, BoardRows);
                col = r.Next(0, BoardCols);
            } while (Mines[row, col] == MINE);
            Mines[row, col] = MINE;
        }

        public static void changeMine(int row, int col)
        {
            generateMine();
            /*
             * Change the value before call the function neighborMinesCount because it'll return the same mine 
             * if the cell is already a mine.
             */
            Mines[row, col] = 0;
            Mines[row, col] = neighborMinesCount(row, col);

            initNeighbors();
        }

        private void gameLoop()
        {
            while (Running)
            {
                update();
                render();
            }

            //clickToNewGame = true;
        }

        private void update()
        {
            checkWin();
        }

        public void render()
        {
            Screen.Invalidate();
        }

        public void stop()
        {
            Running = false;
        }

        public void processClick(MouseEventArgs e)
        {
            Rectangle boardArea = new Rectangle(Ui.StartBoardX, Ui.StartBoardY, Ui.WidthLength, Ui.HeightLength);

            if(!GameOver && !GameWin)
            {
                if (e.Button == MouseButtons.Left && boardArea.Contains(e.Location))
                {
                    Ui.openCell(e, isFirstClick);
                    if (isFirstClick) isFirstClick = false;
                }
                if (e.Button == MouseButtons.Right && boardArea.Contains(e.Location)) {
                    Ui.flagCell(e);
                }
            }

            if (clickToNewGame && !Running && e.Button == MouseButtons.Left)
            {
                newGame(BoardRows, BoardCols, initialMines, Ui.BoxSize);
            }

            if ((GameWin || GameOver) && !clickToNewGame)
            {
                clickToNewGame = true;
                return;
            }
        }

        public static void checkWin()
        {
            int totalArea = BoardRows * BoardCols;

            if(NumMines == 0 && openCellsCount == totalArea - initialMines && flaggedCellsCount == initialMines)
            {
                GameWin = true;
                Running = false;
            }

            if(GameOver)
            {
                Running = false;
            }

            //Debug.WriteLine(string.Format("{0} - {1}, {2}, {3}", GameWin, NumMines, openCellsCount, flaggedCellsCount));
        }

        public void newGame(int boardRows, int boardCols, int numMines, int boxSize)
        {
            //Debug.WriteLine("New Game");

            BoardRows = boardRows;
            BoardCols = boardCols;
            NumMines = numMines;
            initialMines = numMines;
            openCellsCount = 0;
            flaggedCellsCount = 0;

            isFirstClick = true;
            clickToNewGame = false;

            GameOver = false;
            GameWin = false;

            Ui = new UI(boardRows, boardCols, numMines, boxSize);

            Mines = new int[boardRows, boardCols];
            Board = new int[boardRows, boardCols];

            generateMines();
            initNeighbors();

            Running = true;

            printBoard();

            GameLoopThread = new Thread(gameLoop);
            GameLoopThread.Start();
        }

        private static void initNeighbors()
        {
            for (int i = 0; i < Mines.GetLength(0); i++)
            {
                for (int j = 0; j < Mines.GetLength(1); j++)
                {
                    Mines[i, j] = neighborMinesCount(i, j);
                }
            }
        }

        public static void floodFill(int row, int col)
        {
            Point startNode = new Point(col, row);
            Point currentNode;

            Stack<Point> stack = new Stack<Point>();
            stack.Push(startNode);

            while(stack.Count > 0)
            {
                currentNode = stack.Pop();

                if (Mines[currentNode.Y, currentNode.X] == MINE) continue;

                if (currentNode.Y >= 0 && currentNode.Y < BoardRows && currentNode.X >= 0 && currentNode.X < BoardCols)
                {
                    if (Board[currentNode.Y, currentNode.X] != OPENED)
                        openCellsCount++;

                    Board[currentNode.Y, currentNode.X] = OPENED;

                    if (currentNode.Y - 1 >= 0 && Board[currentNode.Y - 1, currentNode.X] == UNOPENED)
                        stack.Push(new Point(currentNode.X, currentNode.Y - 1));

                    if (currentNode.X - 1 >= 0 && Board[currentNode.Y, currentNode.X - 1] == UNOPENED)
                        stack.Push(new Point(currentNode.X - 1, currentNode.Y));

                    if (currentNode.X + 1 < BoardCols && Board[currentNode.Y, currentNode.X + 1] == UNOPENED)
                        stack.Push(new Point(currentNode.X + 1, currentNode.Y));

                    if (currentNode.Y + 1 < BoardRows && Board[currentNode.Y + 1, currentNode.X] == UNOPENED)
                        stack.Push(new Point(currentNode.X, currentNode.Y + 1));
                }
            } // end while

            Screen.Invalidate();
        }

        private static int minesCount(int[,] sub, int rows, int cols)
        {
            int mines = 0;

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    if (sub[i, j] == MINE) mines++;
                }
            }

            return mines;
        }

        private static int neighborMinesCount(int row, int col)
        {

            int[,] sub = new int[3, 3];

            if (Mines[row, col] == MINE) return MINE;

            // Check borders
            if(row == 0 && col == 0) // Upper left corner
            {
                sub[0, 0] = Mines[row, col];
                sub[0, 1] = Mines[row, col + 1];
                sub[1, 0] = Mines[row + 1, col];
                sub[1, 1] = Mines[row + 1, col + 1];
                return minesCount(sub, 2, 2);
            }

            if(row == 0 && col == BoardCols - 1) // Upper right corner
            {
                sub[0, 0] = Mines[row, col - 1];
                sub[0, 1] = Mines[row, col];
                sub[1, 0] = Mines[row + 1, col - 1];
                sub[1, 1] = Mines[row + 1, col];
                return minesCount(sub, 2, 2);
            }

            if (row == BoardRows - 1 && col == 0) // Lower left corner
            {
                sub[0, 0] = Mines[row - 1, col];
                sub[0, 1] = Mines[row - 1, col + 1];
                sub[1, 0] = Mines[row, col];
                sub[1, 1] = Mines[row, col + 1];
                return minesCount(sub, 2, 2);
            }

            if (row == BoardRows - 1 && col == BoardCols - 1) // Lower right corner
            {
                sub[0, 0] = Mines[row - 1, col - 1];
                sub[0, 1] = Mines[row - 1, col];
                sub[1, 0] = Mines[row, col - 1];
                sub[1, 1] = Mines[row, col];
                return minesCount(sub, 2, 2);
            }

            if (row == 0) // Upper corner
            {
                sub = new int[2, 3];
                sub[0, 0] = Mines[row, col - 1];
                sub[0, 1] = Mines[row, col];
                sub[0, 2] = Mines[row, col + 1];
                sub[1, 0] = Mines[row + 1, col - 1];
                sub[1, 1] = Mines[row + 1, col];
                sub[1, 2] = Mines[row + 1, col + 1];
                return minesCount(sub, 2, 3);
            }

            if(row == BoardRows - 1) // Lower corner
            {
                sub[0, 0] = Mines[row - 1, col - 1];
                sub[0, 1] = Mines[row - 1, col];
                sub[0, 2] = Mines[row - 1, col + 1];
                sub[1, 0] = Mines[row, col - 1];
                sub[1, 1] = Mines[row, col];
                sub[1, 2] = Mines[row, col + 1];
                return minesCount(sub, 2, 3);
            }

            if(col == 0) // Left corner
            {
                sub[0, 0] = Mines[row - 1, col];
                sub[0, 1] = Mines[row - 1, col + 1];
                sub[1, 0] = Mines[row, col];
                sub[1, 1] = Mines[row, col + 1];
                sub[2, 0] = Mines[row + 1, col];
                sub[2, 1] = Mines[row + 1, col + 1];
                return minesCount(sub, 3, 2);
            }

            if(col == BoardCols - 1) // Right corner
            {
                sub[0, 0] = Mines[row - 1, col - 1];
                sub[0, 1] = Mines[row - 1, col];
                sub[1, 0] = Mines[row, col - 1];
                sub[1, 1] = Mines[row, col];
                sub[2, 0] = Mines[row + 1, col - 1];
                sub[2, 1] = Mines[row + 1, col];
                return minesCount(sub, 3, 2);
            }

            sub = new int[3, 3];
            sub[0, 0] = Mines[row - 1, col - 1];
            sub[0, 1] = Mines[row - 1, col];
            sub[0, 2] = Mines[row - 1, col + 1];
            sub[1, 0] = Mines[row, col - 1];
            sub[1, 1] = Mines[row, col];
            sub[1, 2] = Mines[row, col + 1];
            sub[2, 0] = Mines[row + 1, col - 1];
            sub[2, 1] = Mines[row + 1, col];
            sub[2, 2] = Mines[row + 1, col + 1];
            return minesCount(sub, 3, 3);

        } // end neighborMinesCount

    } // end class
}