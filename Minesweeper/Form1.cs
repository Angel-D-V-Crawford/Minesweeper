using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {

        private MinesweeperGame game;

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBoxGame_Paint(object sender, PaintEventArgs e)
        {
            Size size = pictureBoxGame.ClientSize;
            game.Ui.render(e.Graphics, size);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            game.stop();
        }

        private void pictureBoxGame_Resize(object sender, EventArgs e)
        {
            pictureBoxGame.Invalidate();
        }

        private void pictureBoxGame_MouseClick(object sender, MouseEventArgs e)
        {
            game.processClick(e);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Invoke(new Action(() => 
            {
                menuStrip1.Refresh();
            }));

            game = new MinesweeperGame(9, 9, 15, 50, pictureBoxGame);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.stop();

            Application.Exit();
        }

        private void toolStripMenuItemGame_MouseLeave(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                menuStrip1.Refresh();
            }));
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 formDifficulty = new Form2(this);
            formDifficulty.ShowDialog();
        }

        public void newGame(int difficulty)
        {
            if (difficulty == (int) Difficulty.EASY)
            {
                Console.WriteLine("Difficulty: Easy");
                resize(new Size(650, 700));
                game.newGame(9, 9, 15, 50);
            }
            if (difficulty == (int) Difficulty.MEDIUM)
            {
                Console.WriteLine("Difficulty: Medium");
                resize(new Size(800, 850));
                game.newGame(16, 16, 40, 40);
            }
            if (difficulty == (int) Difficulty.HARD)
            {
                Console.WriteLine("Difficulty: Hard");
                resize(new Size(1300, 850));
                game.newGame(16, 30, 99, 40);
            }
        }

        private void resize(Size size)
        {
            this.Size = size;
            recenter();
        }

        private void recenter()
        {
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;

            this.Top = (area.Height - this.Height) / 2;
            this.Left = (area.Width - this.Width) / 2;
        }
    } // end class
}
