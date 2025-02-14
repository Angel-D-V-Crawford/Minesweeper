using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form2 : Form
    {
        private Form1 gameForm = null;

        public Form2(Form originForm)
        {
            gameForm = originForm as Form1;

            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            int difficulty = 0;

            if (radioButtonEasy.Checked == true) difficulty = (int) Difficulty.EASY;
            if (radioButtonMedium.Checked == true) difficulty = (int) Difficulty.MEDIUM;
            if (radioButtonHard.Checked == true) difficulty = (int) Difficulty.HARD;

            gameForm.newGame(difficulty);
            Close();
        }
    }
}
