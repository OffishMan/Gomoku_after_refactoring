using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Result.DAL;

namespace Logic
{
    public static class ConnectorToDAL
    {
        private static ResultDAO res;

        static ConnectorToDAL()
        {
            res = new ResultDAO();
        }

        public static int AddResult(string nick, int score)
        {
            return res.Add(nick, score);
        }

        public static List<TableLine> GetTop10(int score)
        {
            return res.ShowPosition(score).ToList<TableLine>();
        }

        public static string GetMyPositionNumber(int score)
        {
            return res.ShowMyPlace(score);
        }

        public static List<TableLine> GetAll()
        {
            return res.ShowAll().ToList<TableLine>();
        }

        public static int GetScore(Board board, int gameSize)
        {
            int count = 0;
            for (int i = 0; i < board.Count; ++i)
            {
                for (int j = 0; j < board.Count; ++j)
                {
                    if (board[i, j] == 0)
                        ++count;
                }
            }
            return (int)Math.Pow(count, gameSize);
        }
    }
}
