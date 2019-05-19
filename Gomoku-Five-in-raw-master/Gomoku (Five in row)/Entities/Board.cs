using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Board
    {
        private List<List<int>> _table;                 //Таблица, -1 - белый камень (первый ход), 1 - чёрный камень, 0 - пустая клетка
        private readonly bool _whoFirst;                //Ключ показывает кто ходит первым, true - машина, значение не изменяется в течение партии.
        private bool _person;                           //True, если ходит человек, меняется в течение игры

        public List<List<int>> Table { get => _table; set => _table = value; }
        public bool Person { get => _person; set => _person = value; }
        public bool WhoFirst => _whoFirst;
        public int Count => _table.Count;

        public int this[int i, int j]
        {
            get
            {
                return Table[i][j];
            }
            set
            {
                Table[i][j] = value;
            }
        }

        public Board(int n, bool whoFirst)
        {
            Table = new List<List<int>>(n);     //Инициализация массива
            for (int i = 0; i < n; i++)
            {
                Table.Add(new List<int>(n));
                for (int j = 0; j < n; j++)
                {
                    Table[i].Add(0);
                }
            }

           _whoFirst = whoFirst;
        }

        public void ClearBoard()
        {
            for (int i = 0; i < Table.Count; i++)
            {
                for (int j = 0; j < Table.Count; j++)
                {
                    Table[i][j] = 0;
                }
            }

        }

        public IEnumerable<Point> AllCells()                //Для метода отрисовки
        {
            for (int i = 0; i < Table.Count; ++i)
            {
                for (int j = 0; j < Table.Count; ++j)
                {
                    yield return new Point(i, j);
                }
            }
        }

        public List<Point> GenerateMoves()
        {
            var moves = new List<Point>();

            for (int i = 0; i < Table.Count; i++)
            {
                for (int j = 0; j < Table.Count; j++)
                {
                    if (_table[i][j] == 0)
                    {
                        moves.Add(new Point(i, j));
                    }
                }
            }

            return moves;
        }

        public void DoMove(Point move)
        {
            if (Table[move.X][move.Y] == 0)
            {
                if (Person)
                    Table[move.X][move.Y] = -1;
                else
                    Table[move.X][move.Y] = 1;

                Person = (Person) ? false : true;   //Смена ходящего после сделанного хода
            }
        }

        public void UndoMove(Point move)
        {
            Table[move.X][move.Y] = 0;
            Person = (Person) ? false : true;   //В идеале отмена хода используется только в алгоритме, поэтому "откат ходящего" тоже должен быть
        }
    }
}
