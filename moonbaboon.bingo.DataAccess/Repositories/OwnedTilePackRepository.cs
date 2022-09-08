using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class OwnedTilePackRepository : IOwnedTilePackRepository
    {
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);

        private const string Table = DbStrings.OwnedTilePackTable;

        private static string sql_select(string from)
        {
            return
                $"SELECT U.{DbStrings.Id}, U.{DbStrings.Username}, U.{DbStrings.Nickname}, U.{DbStrings.ProfilePic}," +
                $"TP.{DbStrings.Id}, TP.{DbStrings.Name}, TP.{DbStrings.PicUrl} " +
                $"FROM {from} " +
                $"JOIN {DbStrings.UserTable} AS U ON {Table}.{DbStrings.OwnerId} = T.{DbStrings.Id} " +
                $"JOIN {DbStrings.TilePackTable} AS TP On {Table}.{DbStrings.TilePackId} = TP.{DbStrings.Id} ";
        }
        
        private static OwnedTilePack ReaderToEnt(MySqlDataReader reader)
        {
            UserSimple owner = new(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
            TilePack tilePack = new(reader.GetString(4), reader.GetString(5), reader.GetString(6));
            return new OwnedTilePack(owner, tilePack);
        }
        
        public Task<List<OwnedTilePack>> GetOwnedTilePacks(string userId)
        {
            throw new System.NotImplementedException();
        }
    }
}