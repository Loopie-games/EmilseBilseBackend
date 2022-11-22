using System.Data;

namespace moonbaboon.bingo.Domain
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}