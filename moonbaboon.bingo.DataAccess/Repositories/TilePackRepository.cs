using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class TilePackRepository : ITilePackRepository
    {
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);
        private const string Table = DbStrings.TilePackTable;
        
        private static TilePack ReaderToEnt(MySqlDataReader reader)
        {
            return new TilePack(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), reader.GetValue(2).ToString());
        }
        
        public async Task<List<TilePack>> FindAll()
        {
            var list = new List<TilePack>();
            await _connection.OpenAsync();
            await using var command = new MySqlCommand($"SELECT * FROM `{DbStrings.TilePackTable}`", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToEnt(reader);
                list.Add(ent);
            }
            await _connection.CloseAsync();
            return list;
        }

        public async Task<List<TilePack>> FindAll_LoggedUser(string userId)
        {
            var list = new List<TilePack>();
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT TilePack.Id, TilePack.Name, TilePack.PicUrl, " +
                $"CASE WHEN OwnedTilePack.OwnerId is Null THEN '0' ELSE '1' END as Owned " +
                $"FROM TilePack " +
                $"LEFT JOIN OwnedTilePack ON OwnedTilePack.TilePackId = TilePack.Id && OwnedTilePack.OwnerId = '{userId}' ", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToEnt(reader);
                ent.IsOwned = Convert.ToBoolean(short.Parse(reader.GetString(3)));
                list.Add(ent);
            }
            await _connection.CloseAsync();
            return list;
        }

        public async Task<TilePack> FindDefault()
        {
            TilePack? ent = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT * FROM {DbStrings.TilePackTable} " +
                $"WHERE {DbStrings.TilePackTable}.{DbStrings.Name} = 'Default' "
                , _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToEnt(reader);
                
            }
            await _connection.CloseAsync();
            return ent ?? throw new Exception("no Default Package found");
        }

        public async Task<TilePack> FindById(string packId)
        {
            TilePack? ent = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT * FROM {DbStrings.TilePackTable} " +
                $"WHERE {DbStrings.TilePackTable}.{DbStrings.Id} = '{packId}' "
                , _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToEnt(reader);
            }
            await _connection.CloseAsync();
            return ent ?? throw new Exception("no tilePackage found with id: " + packId);
        }

        public async Task<TilePack> Create(TilePack toCreate)
        {
            TilePack? ent = null;
            var uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"INSERT INTO {Table} " +
                $"VALUES ('{uuid}','{toCreate.Name}', '{toCreate.PicUrl ?? ""}'); " +
                sql_select(Table) + 
                $"WHERE {Table}.{DbStrings.Id} = '{uuid}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                ent = ReaderToEnt(reader);
            }

            await _connection.CloseAsync();
            return ent ?? throw new Exception($"Error i creating {Table} with name: " + toCreate.Name);
        }

        private static string sql_select(string from)
        {
            return $"SELECT * FROM {from} ";
        }
    }
}