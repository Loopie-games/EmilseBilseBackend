using System.Data;
using moonbaboon.bingo.Domain;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection CreateConnection()
        {
            return new MySqlConnection("Server=185.51.76.157; Database=emilse_bilse_bingo; Uid=root; PWD=jenkis1604; Allow User Variables=true;");
        }
    }
}