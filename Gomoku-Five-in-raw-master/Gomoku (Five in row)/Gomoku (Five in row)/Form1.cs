using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entities;
using Logic;

namespace Gomoku
{
    public partial class Form1 : Form
    {
        private int boardSize;    //Размер игрового поля
        private int cellSize = 100;    //Размер ячейки игрового поля в пикселях
        private int gameSize;     //Количество подряд идущих фигур для победы

        private MoveLogic logic;
        private Board table;
        private bool firstMove;

        public Form1()
        {
            InitializeComponent();

            boardSize = 3;
            gameSize = 3;
            SetClientSizeCore(cellSize * boardSize, cellSize * boardSize);
            table = new Board(boardSize, false);
            logic = new MoveLogic(table, gameSize);
            logic.Person = (logic.WhoFirst) ? false : true;     //Первое определение, сейчас по умолчанию первым ходит пользователь
            firstMove = true;

        }

        public Form1(int size, int movesCount)
        {
            InitializeComponent();

            boardSize = size;
            gameSize = movesCount;
            SetClientSizeCore(cellSize * boardSize, cellSize * boardSize);
            table = new Board(boardSize, false);
            logic = new MoveLogic(table, gameSize);
            logic.Person = (logic.WhoFirst) ? false : true;     //Первое определение, сейчас по умолчанию первым ходит пользователь
            firstMove = true;

        }

        #region Draw

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBoard(e.Graphics);
        }

        void DrawBoard(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (Point location in table.AllCells())
            {
                DrawCell(g, location);
                if (table[location.X, location.Y] == -1)
                    DrawWhiteStone(g, location);
                if (table[location.X, location.Y] == 1)
                    DrawBlackStone(g, location);
            }

        }

        void DrawCell(Graphics g, Point location)
        {
            Rectangle rect = CreateCellRect(location);
            g.FillRectangle(Brushes.Gray, rect);
            g.DrawRectangle(Pens.Silver, rect);
        }

        void DrawWhiteStone(Graphics g, Point location)
        {
            Rectangle rect = CreateStoneRect(location);
            g.FillEllipse(Brushes.White, rect);
            g.DrawEllipse(Pens.Black, rect);
        }

        void DrawBlackStone(Graphics g, Point location)
        {
            g.FillEllipse(Brushes.Black, CreateStoneRect(location));
        }

        Rectangle CreateCellRect(Point location)
        {
            return new Rectangle(location.X * cellSize, location.Y * cellSize, cellSize, cellSize);
        }

        Rectangle CreateStoneRect(Point location)
        {
            Rectangle result = CreateCellRect(location);
            result.Inflate(-3, -3);
            return result;
        }

        #endregion

        #region Action

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left && logic.Person)
            {
                Point cellCoords = new Point(e.X / cellSize, e.Y / cellSize);
                if (table[cellCoords.X, cellCoords.Y] == 0)
                {
                    MakeMove(cellCoords);       //Ход человека                                       

                    if (firstMove)
                    {
                        MakeMove(logic.GenerateFirstMove(cellCoords));
                        firstMove = false;
                    }
                    else
                        MakeMove(logic.StartAlgorithm());

                }
            }
        }

        void MakeMove(Point location)
        {
            if (location == new Point(-1, -1))
            {
                MessageBox.Show("Ничья!");
                table.ClearBoard();
                Refresh();
                Close();
            }
            else
            {
                table.DoMove(new Point(location.X, location.Y));      
                Refresh();

                logic.Person = (logic.Person) ? false : true;     //Так как ход сделан, то "ходящий" уже сменился

                if (logic.ScoreFunction(new Point(location.X, location.Y)) >= 1000000000)
                {
                    if (table[location.X, location.Y] == 1)
                    {
                        MessageBox.Show("Вы проиграли.");
                    }
                    else
                    {
                        try
                        {
                            string nick = "";
                            int score = ConnectorToDAL.GetScore(table, gameSize);
                            int id;
                            Form3 form3 = new Form3();
                            form3.ShowDialog();
                            if (form3.DialogResult == DialogResult.OK)
                            {
                                nick = form3.ReturnNickname();
                                id = ConnectorToDAL.AddResult(nick, score);
                                MessageBox.Show($"Вы победили и заняли {ConnectorToDAL.GetMyPositionNumber(score)} из {id} возможных!\nВаши очки за победу: {score}");
                                //List<TableLine> list = ConnectorToDAL.GetAll();
                            }
                            else
                            {
                                MessageBox.Show("Вы победили!");
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Результат не может быть сохранён, так как недоступна база данных. Но Вы всё равно победитель!");
                        }
                    }

                    table.ClearBoard();
                    Refresh();
                    Close();
                }

                logic.Person = (logic.Person) ? false : true;     
            }
        }
        #endregion

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {            
            this.Owner.Show();
            this.Close();            
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                Close();
            }
        }
    }
}
