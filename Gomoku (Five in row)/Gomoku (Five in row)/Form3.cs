using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logic;

namespace Gomoku
{
    public partial class Form3 : Form
    {
        //private int scr;

        public Form3(/*int score*/)
        {
            //scr = score;
            InitializeComponent();
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public string ReturnNickname()
        {
            return textBox1.Text;
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
    }
}
