using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Result.DAL;

namespace Logic
{
    public class MoveLogic
    {
        private Board _table;
        private int _gameSize;

        private Board Table { get => _table; set => _table = value; }
        public bool WhoFirst => _table.WhoFirst;
        public bool Person { get => _table.Person; set => _table.Person = value; }
        public int GameSize { get => _gameSize; set => _gameSize = value; }

        public MoveLogic(int n, bool whoFirst, int gameSize)
        {
            _table = new Board(n, whoFirst);
            GameSize = gameSize;
        }

        public MoveLogic(Board table, int gameSize)
        {
            _table = table;
            GameSize = gameSize;
        }

   
        public Point StartAlgorithm()
        {
            KeyValuePair<Point, int> bestMove = new KeyValuePair<Point, int>(new Point(-1, -1), -2000000);
            
            int localScore = 0;

            var moves = Table.GenerateMoves();

            if (moves.Count == 0)           //Принудительная генерация первого хода, если комп ещё не ходил
                return new Point(-1, -1);

            foreach (var item in moves)
            {
                Table.DoMove(item);         //Делается ход
                Person = (Person) ? false : true;   //Ходящий меняется с пользователя на пк
                localScore = ScoreFunction(item);   //Оценка хода
                Person = (Person) ? false : true;   //Обратная смена
                if (bestMove.Value < localScore)    //Ход запоминается, если он удачнее предыдущих
                {
                    bestMove = new KeyValuePair<Point, int>(new Point(item.X, item.Y), localScore);
                }
                Table.UndoMove(item);       //Отмена хода
            }

            return bestMove.Key;
        }

        public Point GenerateFirstMove(Point move)
        {
            var possibleMoves = new List<Point>();
            if (move.X > 0 && move.Y > 0)
                possibleMoves.Add(new Point(move.X - 1, move.Y - 1));

            if (move.X < Table.Count - 1 && move.Y > 0)
                possibleMoves.Add(new Point(move.X + 1, move.Y - 1));

            if (move.X < Table.Count - 1 && move.Y < Table.Count - 1)
                possibleMoves.Add(new Point(move.X + 1, move.Y + 1));

            if (move.X > 0 && move.Y < Table.Count - 1)
                possibleMoves.Add(new Point(move.X - 1, move.Y + 1));
            var temp = new Random();

            return possibleMoves[temp.Next(0, possibleMoves.Count)];
        }

        public List<int> GenerateBreakingPoint(Point move)      //Будем проверять от точки, в которую сходили так как оцениваем прерывание цепочки с её позиции
        {                                                       //ДОБАВИТЬ УСЛОВИЕ ПРОВЕРКИ НА ПРЕДЫДУЩИЙ ЗНАК
            var list = new List<int>();
            for (int i = 0; i < 8; i++) //0-4 1-5 2-6 3-7
            {
                list.Add(0);
            }

            for (int i = 1; i < GameSize + 1; i++)  //Идём от 1, потому что нет смысла смотреть ту же клетку, в которую мы сходили
            {       //Тут нужна проверка типа list[0] + 1 == i, потому что незачем в принципе считать сколько фишек противника мы можем закрыть с учётом пропуска
                if (move.X - i >= 0)
                {
                    if (list[0] + 1 == i)
                    {
                        if (Table[move.X - i, move.Y] == -1 && list[0] == 0)        //0
                            ++list[0];
                        else
                            if (Table[move.X - i, move.Y] == -1 && list[0] > 0)       //Иначе гарантированно у нас был уже ход от человека
                            ++list[0];                        
                    }
                    if (move.Y - i >= 0)     //Верхняя часть главной диагонали   1
                    {
                        if (list[1] + 1 == i)
                        {
                            if (Table[move.X - i, move.Y - i] == -1 && list[1] == 0)
                                ++list[1];
                            else
                                if (Table[move.X - i, move.Y - i] == -1 && list[1] > 0)       //Иначе гарантированно у нас был уже ход от человека
                                ++list[1];
                        }
                    }
                    if (move.Y + i < Table.Count)   //Нижняя часть побочной диагонали   7
                    {
                        if (list[7] + 1 == i)
                        {
                            if (Table[move.X - i, move.Y + i] == -1 && list[7] == 0)
                                ++list[7];
                            else
                                if (Table[move.X - i, move.Y + i] == -1 && list[7] > 0)       //Иначе гарантированно у нас был уже ход от человека
                                ++list[7];
                        }
                    }
                }
                if (move.X + i < Table.Count)
                {
                    if (list[4] + 1 == i)       //4
                    {
                        if (Table[move.X + i, move.Y] == -1 && list[4] == 0)
                            ++list[4];
                        else
                            if (Table[move.X + i, move.Y] == -1 && list[4] > 0)       //Иначе гарантированно у нас был уже ход от человека
                            ++list[4];
                    }                    
                    if (move.Y - i >= 0)     //Верхняя часть побочной диагонали  3
                    {
                        if (list[3] + 1 == i)
                        {
                            if (Table[move.X + i, move.Y - i] == -1 && list[3] == 0)
                                ++list[3];
                            else
                                if (Table[move.X + i, move.Y - i] == -1 && list[3] > 0)       //Иначе гарантированно у нас был уже ход от человека
                                ++list[3];
                        }
                    }
                    if (move.Y + i < Table.Count)   //Нижняя часть главной диагонали    5
                    {
                        if (list[5] + 1 == i)
                        {
                            if (Table[move.X + i, move.Y + i] == -1 && list[5] == 0)
                                ++list[5];
                            else
                                if (Table[move.X + i, move.Y + i] == -1 && list[5] > 0)       //Иначе гарантированно у нас был уже ход от человека
                                ++list[5];
                        }
                    }
                }
                if (move.Y - i >= 0)     //2 проверки для вертикали  2
                {
                    if (list[2] + 1 == i)
                    {
                        if (Table[move.X, move.Y - i] == -1 && list[2] == 0)
                            ++list[2];
                        else
                            if (Table[move.X, move.Y - i] == -1 && list[2] > 0)       //Иначе гарантированно у нас был уже ход от человека
                            ++list[2];
                    }
                }
                if (move.Y + i < Table.Count)   //6
                {
                    if (list[6] + 1 == i)
                    {
                        if (Table[move.X, move.Y + i] == -1 && list[6] == 0)
                            ++list[6];
                        else
                            if (Table[move.X, move.Y + i] == -1 && list[6] > 0)       //Иначе гарантированно у нас был уже ход от человека
                            ++list[6];                        
                    }
                }
            }

            var temp = new List<int>();

            temp.Add(list[0] + list[4]);
            temp.Add(list[1] + list[5]);
            temp.Add(list[2] + list[6]);
            temp.Add(list[3] + list[7]);

            return temp;
        }

        public int ScoreFunction(Point move)
        {
            var count = new List<int>(4);         //Количество подряд идущих фигур, 1 - по горизонтали, 
            //2 - нисходящая горизонталь слева направо, 3 - по вертикали, 4 - низходящая диагональ справа налево


            var freeSpace = new List<int>(4);     //Количество свободного места между ходами
            //Предполагается, что при 1-2 пустом месте его можно засчитать за дополнительный ход

            var openSpace = new List<int>(4);       //По идее должно показывать есть ли свободной место от данной позиции, то есть конструкция вида о_хххх или о_хххх_о. 1 - свободное место с 1 стороны, 2 - с обеих сторон

            var resultScore = new List<int>(4);

            var beginningPoint = new List<Point>(4);

            var breakingPoint = new List<int>(GenerateBreakingPoint(move));

            for (int i = 0; i < 4; i++)         //Тут я ошибся, в идеальном случае в данной программе нет ни малейшего смысла использовать List
            {                                   //Гораздо лучше и проще (и быстрее по идее) будет работа с простым двумерным массивом, так как у нас не стоит цели динамически изменять размер
                count.Add(0);
                freeSpace.Add(0);
                openSpace.Add(0);
                resultScore.Add(0);
                beginningPoint.Add(new Point(0, 0));
            }

            int sign = (Table.Person) ? -1 : 1;        //Чтобы не злоупотреблять проверками, переключателями и излишним кодом, автоматическая смена
            //int antiSign = -sign;

            int level = 0;

            if (GameSize == 5)
                level = 100;
            else
                if (GameSize == 4)
                level = 500;
            else
                if (GameSize == 3)
                level = 1000;

            int temp;

            beginningPoint[0] = new Point((move.X - GameSize > 0) ? (move.X - GameSize) : 0, move.Y);   //Инициализация точки для проверки по горизонтали

            temp = Math.Min(move.X, move.Y);              //Инициализация точки для проверки по главной диагонали
            if (temp > GameSize)
                beginningPoint[1] = new Point(move.X - GameSize, move.Y - GameSize);
            else
                beginningPoint[1] = new Point(move.X - temp, move.Y - temp);

            beginningPoint[2] = new Point(move.X, (move.Y - GameSize > 0) ? (move.Y - GameSize) : 0);  //Инициализация точки для проверки по вертикали 

            temp = Math.Min(Table.Count - move.X - 1, move.Y);
            if (temp > GameSize)                                       //Инициализация точки для проверки по побочной диагонали
                beginningPoint[3] = new Point(move.X + GameSize, move.Y - GameSize);
            else
                beginningPoint[3] = new Point(move.X + temp, move.Y - temp);

            

            HorizontalValueEstimate(beginningPoint, freeSpace,openSpace, sign, count);

            MainDiagonalValueEstimate(beginningPoint, freeSpace, openSpace, sign, count);

            VerticalValueEstimate(beginningPoint, freeSpace, openSpace, sign, count);

            SideDiagonalValueEstimate(beginningPoint, freeSpace, openSpace, sign, count);



            if (count[0] >= GameSize || count[1] >= GameSize || count[2] >= GameSize || count[3] >= GameSize)
            {
                return 1000000000;
            }

            for (int i = 0; i < 4; i++)     //Чтобы был приоритет для создания "свободного" места и правильный учёт хода.
            {
                if (freeSpace[i] > 0)
                    count[i] += 1;
            }

            for (int i = 0; i < 4; i++)     //Вот тут по идее производится рассчёт наиболее приятных исходов
            {
                if (count[i] >= (GameSize - 1) && freeSpace[i] >= 1 && openSpace[i] != 0)
                    resultScore[i] = 75000 + (int)Math.Pow(level, breakingPoint[i]);
                else
              if (count[i] >= (GameSize - 1) && freeSpace[i] >= 1)
                    resultScore[i] = 60000 + (int)Math.Pow(level, breakingPoint[i]);
                else
                    if (count[i] == (GameSize - 1) && openSpace[i] != 0)
                    resultScore[i] = 40000 + (int)Math.Pow(level, breakingPoint[i]);
                else
              if (count[i] == (GameSize - 2) && freeSpace[i] >= 2 && openSpace[i] != 0)
                    resultScore[i] = 30000 + (int)Math.Pow(level, breakingPoint[i]);
                else
              if (count[i] == (GameSize - 2) && (freeSpace[i] >= 2 || (freeSpace[i] + openSpace[i]) == 3))
                    resultScore[i] = 25000 + (int)Math.Pow(level, breakingPoint[i]);
                else
              if (count[i] == (GameSize - 3) && (freeSpace[i] > 2))
                    resultScore[i] = 10000 + (int)Math.Pow(level, breakingPoint[i]);
                else
              if (count[i] >= (GameSize - 4) && (freeSpace[i] + openSpace[i]) > 3)
                    resultScore[i] = 100 + (int)Math.Pow(level, breakingPoint[i]);
                else
                    resultScore[i] = 10 * count[i] + (int)Math.Pow(level, breakingPoint[i]);
            }

            return (resultScore[0] + resultScore[1] + resultScore[2] + resultScore[3]);     //Если результат не выигрышный, то возвращается оценка для этой точки, она может быть более ста тысяч
        }

        public void HorizontalValueEstimate(List<Point> beginningPoint, List<int> freeSpace, List<int> openSpace, int sign, List<int> count)
        {
            int temp = ((beginningPoint[0].X + (GameSize * 2 + 1) >= Table.Count) ? Table.Count : (beginningPoint[0].X + (GameSize * 2 + 1)));
            for (int i = beginningPoint[0].X; i < temp; i++)    //Пробегаемся по горизонтали
            {
                if (Table[i, beginningPoint[0].Y] == 0)        //Если текущее значение 0, то проверяем были ли нужные значения до и после и решаем freeSpace или openSpace это.
                {
                    if (i > 0 && (i + 1) < Table.Count)                   //Если мы не на крайней позиции поля, то 
                    {
                        if (Table[i + 1, beginningPoint[0].Y] == sign
                            && Table[i - 1, beginningPoint[0].Y] == sign) //Если следующий и предыдущий символы нужные нам, то это свободное место
                        {
                            ++freeSpace[0];
                        }
                        else
                            if (Table[i + 1, beginningPoint[0].Y] == sign)  //Так как идём слева направо и предыдущее условие нам не понравилось, то проверяем на открытость последовательности
                        {
                            ++openSpace[0];
                        }
                        else
                            if (Table[i - 1, beginningPoint[0].Y] == sign)  //Как и для предыдущего случая, но проверяем открытость справа
                        {
                            ++openSpace[0];
                        }
                    }
                    else
                        if ((i > 0 && Table[i - 1, beginningPoint[0].Y] == sign)
                        || ((i + 1) < Table.Count && Table[i + 1, beginningPoint[0].Y] == sign)) //Если мы находимся на границе таблицы, то у нас может быть только открытая комбинация, проверяем это
                    {
                        openSpace[0] = 1;
                    }
                }

                if (Table[i, beginningPoint[0].Y] == sign)      //Если найден нужный символ, то засчитываем его
                {
                    if (count[0] == 0)
                        ++count[0];
                    else
                        if (Table[i - 1, beginningPoint[0].Y] == sign)       //Иначе будет считать каждый отдельно стоящий знак, если не встретит знака противника
                        ++count[0];
                }
                
                if (Table[i, beginningPoint[0].Y] != sign
                    && Table[i, beginningPoint[0].Y] != 0
                    && (count[0] + openSpace[0] + freeSpace[0]) < GameSize)  //Глобальная проверка на сброс показателей
                {       //Если исследуемая клетка не пустая и не "наш" знак, а также, если сумма пустых клеток и последовательности меньше 5, то невозможно получить выигрышную комбинацию и мы о ней забываем
                    count[0] = 0;
                    openSpace[0] = 0;
                    freeSpace[0] = 0;
                    if ((temp - i) < GameSize)     //Если встретился мешающий символ и при этом количество клеток остаётся меньше 5, то победную последовательность не найти и цикл стоит прервать
                    {
                        break;
                    }
                }
            }
        }

        public void VerticalValueEstimate(List<Point> beginningPoint, List<int> freeSpace, List<int> openSpace, int sign, List<int> count)
        {
            int temp = ((beginningPoint[2].Y + (GameSize * 2 + 1) >= Table.Count) ? Table.Count : (beginningPoint[2].Y + (GameSize * 2 + 1)));
            for (int i = beginningPoint[2].Y; i < temp; i++)    //Пробегаемся по вертикали
            {
                if (Table[beginningPoint[2].X, i] == 0)        //Если текущее значение 0, то проверяем были ли нужные значения до и после и решаем freeSpace или openSpace это.
                {
                    if (i > 0 && (i + 1) < Table.Count)                   //Если мы не на крайней позиции поля, то 
                    {
                        if (Table[beginningPoint[2].X, i + 1] == sign
                            && Table[beginningPoint[2].X, i - 1] == sign) //Если следующий и предыдущий символы нужные нам, то это свободное место
                        {
                            ++freeSpace[2];
                        }
                        else
                            if (Table[beginningPoint[2].X, i + 1] == sign)  //Так как идём слева направо и предыдущее условие нам не понравилось, то проверяем на открытость последовательности
                        {
                            ++openSpace[2];
                        }
                        else
                            if (Table[beginningPoint[2].X, i - 1] == sign)  //Как и для предыдущего случая, но проверяем открытость справа
                        {
                            ++openSpace[2];
                        }
                    }
                    else
                        if ((i > 0 && Table[beginningPoint[2].X, i - 1] == sign)
                        || ((i + 1) < Table.Count && Table[beginningPoint[2].X, i + 1] == sign)) //Если мы находимся на границе таблицы, то у нас может быть только открытая комбинация, проверяем это
                    {
                        openSpace[2] = 1;
                    }
                }

                if (Table[beginningPoint[2].X, i] == sign)      //Если найден нужный символ, то засчитываем его
                {
                    if (count[2] == 0)
                        ++count[2];
                    else
                        if (Table[beginningPoint[2].X, i - 1] == sign)       //Иначе будет считать каждый отдельно стоящий знак, если не встретит знака противника
                        ++count[2];
                }

                if (Table[beginningPoint[2].X, i] != sign
                    && Table[beginningPoint[2].X, i] != 0
                    && (count[2] + openSpace[2] + freeSpace[2]) < GameSize)  //Глобальная проверка на сброс показателей
                {       //Если исследуемая клетка не пустая и не "наш" знак, а также, если сумма пустых клеток и последовательности меньше 5, то невозможно получить выигрышную комбинацию и мы о ней забываем
                    count[2] = 0;
                    openSpace[2] = 0;
                    freeSpace[2] = 0;
                    if ((temp - i) < GameSize)     //Если встретился мешающий символ и при этом количество клеток остаётся меньше 5, то победную последовательность не найти и цикл стоит прервать
                    {
                        break;
                    }
                }
            }
        }

        public void MainDiagonalValueEstimate(List<Point> beginningPoint, List<int> freeSpace, List<int> openSpace, int sign, List<int> count)
        {
            int temp = ((Table.Count - beginningPoint[1].X - (GameSize * 2 + 1)) > 0 && (Table.Count - beginningPoint[1].Y - (GameSize * 2 + 1)) > 0) ? (GameSize * 2 + 1) : Math.Min(Table.Count - beginningPoint[1].X, Table.Count - beginningPoint[1].Y);
            //Вычисляем МИНИМАЛЬНОЕ расстояние до границы игрового поля
            //Если это расстояние меньше GameSize, то присваиваем сразу 0 ибо это бесперспективная ветка, иначе, если расстояние меньше GameSize*2, но больше GameSize - 1, идём от начальной точки до границы игрового поля
            if (temp >= GameSize)
            {
                for (int i = 0; i < temp; i++)    //Пробегаемся по главной диагонали
                {
                    if (Table[beginningPoint[1].X + i, beginningPoint[1].Y + i] == 0)        //Если текущее значение 0, то проверяем были ли нужные значения до и после и решаем freeSpace или openSpace это.
                    {
                        if ((beginningPoint[1].X + i) > 0
                            && (beginningPoint[1].Y + i) > 0
                            && (beginningPoint[1].X + i + 1) < Table.Count
                            && (beginningPoint[1].Y + i + 1) < Table.Count)                   //Если мы не на крайней позиции поля, то 
                        {
                            if (Table[beginningPoint[1].X + i + 1, beginningPoint[1].Y + i + 1] == sign
                                && Table[beginningPoint[1].X + i - 1, beginningPoint[1].Y + i - 1] == sign) //Если следующий и предыдущий символы нужные нам, то это свободное место
                            {
                                ++freeSpace[1];
                            }
                            else
                                if (Table[beginningPoint[1].X + i + 1, beginningPoint[1].Y + i + 1] == sign)  //Так как идём слева направо и предыдущее условие нам не понравилось, то проверяем на открытость последовательности
                            {
                                ++openSpace[1];
                            }
                            else
                                if (Table[beginningPoint[1].X + i - 1, beginningPoint[1].Y + i - 1] == sign)  //Как и для предыдущего случая, но проверяем открытость справа
                            {
                                ++openSpace[1];
                            }
                        }
                        else
                            if (((beginningPoint[1].X + i) > 0
                            && (beginningPoint[1].Y + i) > 0
                            && Table[beginningPoint[1].X + i - 1, beginningPoint[1].Y + i - 1] == sign)
                            || ((beginningPoint[1].X + i + 1) < Table.Count
                            && (beginningPoint[1].Y + i + 1) < Table.Count
                            && Table[beginningPoint[1].X + i + 1, beginningPoint[1].Y + i + 1] == sign))
                        {    //Если мы находимся на границе таблицы, то у нас может быть только открытая комбинация, проверяем это
                            openSpace[1] = 1;
                        }
                    }

                    if (Table[beginningPoint[1].X + i, beginningPoint[1].Y + i] == sign)      //Если найден нужный символ, то засчитываем его
                    {
                        if (count[1] == 0)
                            ++count[1];
                        else
                         if (Table[beginningPoint[1].X + i - 1, beginningPoint[1].Y + i - 1] == sign)       //Иначе будет считать каждый отдельно стоящий знак, если не встретит знака противника
                            ++count[1];
                    }

                    //if (Table[beginningPoint[1].X + i, beginningPoint[1].Y + i] == -sign)      //Если найден нужный символ, то засчитываем его
                    //{
                    //    if (breakingPoint[1] == 0)
                    //        ++breakingPoint[1];
                    //    else
                    //     if (Table[beginningPoint[1].X + i - 1, beginningPoint[1].Y + i - 1] == -sign)       //Иначе будет считать каждый отдельно стоящий знак, если не встретит знака противника
                    //        ++breakingPoint[1];
                    //}

                    if (Table[beginningPoint[1].X + i, beginningPoint[1].Y + i] != sign
                        && Table[beginningPoint[1].X + i, beginningPoint[1].Y + i] != 0
                        && (count[1] + openSpace[1] + freeSpace[1]) < GameSize)  //Глобальная проверка на сброс показателей
                    {       //Если исследуемая клетка не пустая и не "наш" знак, а также, если сумма пустых клеток и последовательности меньше 5, то невозможно получить выигрышную комбинацию и мы о ней забываем
                        count[1] = 0;
                        openSpace[1] = 0;
                        freeSpace[1] = 0;
                        if ((temp - i) < GameSize)     //Если встретился мешающий символ и при этом количество клеток остаётся меньше 5, то победную последовательность не найти и цикл стоит прервать
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void SideDiagonalValueEstimate(List<Point> beginningPoint, List<int> freeSpace, List<int> openSpace, int sign, List<int> count)
        {
            int temp = ((beginningPoint[3].X - (GameSize * 2 + 1)) > 0 && (Table.Count - beginningPoint[3].Y - (GameSize * 2 + 1)) > 0) ? (GameSize * 2 + 1) : Math.Min(beginningPoint[3].X + 1, Table.Count - beginningPoint[3].Y);
            if (temp >= GameSize)
            {
                for (int i = 0; i < temp; i++)    //Пробегаемся по побочной диагонали
                {
                    if (Table[beginningPoint[3].X - i, beginningPoint[3].Y + i] == 0)        //Если текущее значение 0, то проверяем были ли нужные значения до и после и решаем freeSpace или openSpace это.
                    {
                        if ((beginningPoint[3].X - i) > 0
                            && (beginningPoint[3].Y + i) > 0
                            && (beginningPoint[3].X - i + 1) < Table.Count
                            && (beginningPoint[3].Y + i + 1) < Table.Count)                   //Если мы не на крайней позиции поля, то 
                        {
                            if (Table[beginningPoint[3].X - i - 1, beginningPoint[3].Y + i + 1] == sign
                                && Table[beginningPoint[3].X - i + 1, beginningPoint[3].Y + i - 1] == sign) //Если следующий и предыдущий символы нужные нам, то это свободное место
                            {
                                ++freeSpace[3];
                            }
                            else
                                if (Table[beginningPoint[3].X - i - 1, beginningPoint[3].Y + i + 1] == sign)  //Так как идём справа налево и предыдущее условие нам не понравилось, то проверяем на открытость последовательности
                            {
                                ++openSpace[3];
                            }
                            else
                                if (Table[beginningPoint[3].X - i + 1, beginningPoint[3].Y + i - 1] == sign)  //Как и для предыдущего случая, но проверяем открытость "справа" (так как диагональ побочная, то слева)
                            {
                                ++openSpace[3];
                            }
                        }
                        else
                            if ((beginningPoint[3].Y + i) > 0
                            && (beginningPoint[3].X - i + 1) < Table.Count
                            && Table[beginningPoint[3].X - i + 1, beginningPoint[3].Y + i - 1] == sign
                            || (beginningPoint[3].X - i) > 0
                            && (beginningPoint[3].Y + i + 1) < Table.Count
                            && Table[beginningPoint[3].X - i - 1, beginningPoint[3].Y + i + 1] == sign)
                        {    //Если мы находимся на границе таблицы, то у нас может быть только открытая комбинация, проверяем это
                            openSpace[3] = 1;
                        }
                    }

                    if (Table[beginningPoint[3].X - i, beginningPoint[3].Y + i] == sign)      //Если найден нужный символ, то засчитываем его
                    {
                        if (count[3] == 0)
                            ++count[3];
                        else
                         if (Table[beginningPoint[3].X - i + 1, beginningPoint[3].Y + i - 1] == sign)       //Иначе будет считать каждый отдельно стоящий знак, если не встретит знака противника
                            ++count[3];
                    }

                    if (Table[beginningPoint[3].X - i, beginningPoint[3].Y + i] != sign
                        && Table[beginningPoint[3].X - i, beginningPoint[3].Y + i] != 0
                        && (count[3] + openSpace[3] + freeSpace[3]) < GameSize)  //Глобальная проверка на сброс показателей
                    {       //Если исследуемая клетка не пустая и не "наш" знак, а также, если сумма пустых клеток и последовательности меньше 5, то невозможно получить выигрышную комбинацию и мы о ней забываем
                        count[3] = 0;
                        openSpace[3] = 0;
                        freeSpace[3] = 0;
                        if ((temp - i) < GameSize)     //Если встретился мешающий символ и при этом количество клеток остаётся меньше 5, то победную последовательность не найти и цикл стоит прервать
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
