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
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);
        
        private TilePack ReaderToTilePack(MySqlDataReader reader)
        {
            return new TilePack(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());
        }
        
        public async Task<List<TilePack>> FindAll()
        {
            var list = new List<TilePack>();
            await _connection.OpenAsync();
            await using var command = new MySqlCommand($"SELECT * FROM `{DBStrings.TilePackTable}`", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToTilePack(reader);
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
                $"SELECT * FROM {DBStrings.TilePackTable} " +
                $"WHERE {DBStrings.TilePackTable}.{DBStrings.Name} = 'Default' "
                , _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToTilePack(reader);
                
            }
            await _connection.CloseAsync();
            return ent ?? throw new Exception("no Default Package found");
        }

        public async Task<TilePack> FindById(string packId)
        {
            TilePack? ent = null;
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(
                $"SELECT * FROM {DBStrings.TilePackTable} " +
                $"WHERE {DBStrings.TilePackTable}.{DBStrings.Id} = '{packId}' "
                , _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToTilePack(reader);
            }
            await _connection.CloseAsync();
            return ent ?? throw new Exception("no tilePackage found with id: " + packId);
        }
    }
}