using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gomoku
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int size = 0;
            int movesCount = 0;
            try
            {
                size = int.Parse(textBox2.Text);
                movesCount = int.Parse(textBox1.Text);
            }
            catch 
            {
                MessageBox.Show("Введены некорректные значения. Убедитесь, что введены числа");
            }
            
            if (size >= movesCount && movesCount >= 3)
            {
                Form1 gamePage = new Form1(size, movesCount);
                Hide();
                gamePage.Show();
                gamePage.Owner = this;
            }
            else
            {
                MessageBox.Show("Введены некорректные значения. Игровое поле не может быть меньше количества подряд идущих камней, требуемых для победы");
                
            }
        }        

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int size = 3;
            int movesCount = 3;
            Form1 gamePage = new Form1(size, movesCount);
            Hide();
            gamePage.Show();
            gamePage.Owner = this;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int size = 5;
            int movesCount = 4;
            Form1 gamePage = new Form1(size, movesCount);
            Hide();
            gamePage.Show();
            gamePage.Owner = this;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int size = 7;
            int movesCount = 5;
            Form1 gamePage = new Form1(size, movesCount);
            Hide();
            gamePage.Show();
            gamePage.Owner = this;
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textBox2.Text = "";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textBox1.Text = "";
        }
    }
}
