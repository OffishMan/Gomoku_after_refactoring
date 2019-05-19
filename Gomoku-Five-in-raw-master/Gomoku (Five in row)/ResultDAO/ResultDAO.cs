using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Result.DAL
{
    public class ResultDAO
    {
        private readonly string _connectionString;

        public ResultDAO()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ScoreTable"].ConnectionString;
        }

        public int Add(string nickname, int score)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "AddResult";            //Имя хранимой процедуры

                var nick = new SqlParameter("@Nickname", SqlDbType.NVarChar)
                {
                    Value = nickname
                };
                command.Parameters.Add(nick);
                
                var scr = new SqlParameter("@score", SqlDbType.Int)
                {
                    Value = score
                };
                command.Parameters.Add(scr);
                connection.Open();                
                return (int)(decimal)command.ExecuteScalar();
            }
        }

        public IEnumerable<TableLine> ShowPosition(int score)
        {
            using (var connetion = new SqlConnection(_connectionString))
            {
                var command = connetion.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ShowMyPosition";

                var scr = new SqlParameter("@score", SqlDbType.Int)
                {
                    Value = score
                };
                command.Parameters.Add(scr);
                connetion.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    yield return new TableLine()
                    {
                        Place = reader["pos"].ToString(),
                        Nickname = (string)reader["nick"],
                        Score = (int)reader["scr"]
                    };
                }
            }            
        }

        public IEnumerable<TableLine> ShowAll()
        {
            using (var connetion = new SqlConnection(_connectionString))
            {
                var command = connetion.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ShowAllResults";

                connetion.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    yield return new TableLine()
                    {
                        Place = reader["id"].ToString(),
                        Nickname = (string)reader["Nickname"],
                        Score = (int)reader["Score"]
                    };
                }
            }
        }

        public string ShowMyPlace(int score)
        {
            using (var connetion = new SqlConnection(_connectionString))
            {
                var command = connetion.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ShowPlace";

                var scr = new SqlParameter("@score", SqlDbType.Int)
                {
                    Value = score
                };
                command.Parameters.Add(scr);
                connetion.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return reader["pos"].ToString();
                }
            }
            return null;
        }

    }
}
