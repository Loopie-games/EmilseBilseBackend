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

        private const string Table = DBStrings.PackTileTable;

        private static string sql_select(string from)
        {
            return
                $"SELECT T.{DBStrings.Id}, T.{DBStrings.Action}, TP.{DBStrings.Id}, TP.{DBStrings.Name}, TP.{DBStrings.PicUrl} " +
                $"FROM {from} " +
                $"JOIN {DBStrings.TileTable} AS T ON {Table}.{DBStrings.TileId} = T.{DBStrings.Id} " +
                $"JOIN {DBStrings.TilePackTable} AS TP On {Table}.{DBStrings.PackId} = TP.{DBStrings.Id} ";
        }
        
        private static PackTile ReaderToEnt(MySqlDataReader reader)
        {
            TilePack tilePack = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
            PackTile packTile = new(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), tilePack);
            return packTile;
        }
        
        
        public async Task<List<PackTile>> GetByPackId(string packId)
        {
            List<PackTile> list = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE {Table}.{DBStrings.PackId} = '{packId}';", 
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(ReaderToEnt(reader));

            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<PackTile> Create(PackTile toCreate)
        {
            PackTile? ent = null;
            var uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"INSERT INTO {DBStrings.TileTable} " +
                $"VALUES ('{uuid}', '{toCreate.Action}');" +
                $"INSERT INTO {Table} " +
                $"VALUES ('{uuid}','{toCreate.Pack.Id}'); " +
                sql_select(Table) + 
                $"WHERE {Table}.{DBStrings.TileId} = '{uuid}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                ent = ReaderToEnt(reader);
            }

            await _connection.CloseAsync();
            return ent ?? throw new Exception("Error i creating packtile with action: " + toCreate.Action);
        }

        public async Task<PackTile> GetById(string id)
        {
            PackTile? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(Table) + 
                $"WHERE {Table}.{DBStrings.TileId} = '{id}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                ent = ReaderToEnt(reader);
            }

            await _connection.CloseAsync();
            return ent ?? throw new Exception("Error i creating packtile with Id: " + id);
        }
    }
}