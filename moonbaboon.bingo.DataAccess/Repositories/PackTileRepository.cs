using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class PackTileRepository: IPackTileRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);
        
        private static string sql_select(string from)
        {
            return
                $"SELECT T.{DBStrings.Id}, T.{DBStrings.Action}, TP.{DBStrings.Id}, TP.{DBStrings.Name} " +
                $"FROM {from} " +
                $"JOIN {DBStrings.TileTable} AS T ON {DBStrings.PackTileTable}.{DBStrings.TileId} = T.{DBStrings.Id} " +
                $"JOIN {DBStrings.TilePackTable} AS TP On {DBStrings.PackTileTable}.{DBStrings.PackId} = TP.{DBStrings.Id} ";
        }
        
        private static PackTile ReaderToEnt(MySqlDataReader reader)
        {
            TilePack tilePack = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString());
            PackTile packTile = new(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), tilePack);
            return packTile;
        }
        
        
        public async Task<List<PackTile>> GetByPackId(string packId)
        {
            List<PackTile> list = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(DBStrings.PackTileTable) +
                $"WHERE {DBStrings.PackTileTable}.{DBStrings.PackId} = '{packId}';", 
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(ReaderToEnt(reader));

            }

            await _connection.CloseAsync();
            return list;
        }
    }
}