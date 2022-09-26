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
        private const string Table = DbStrings.TilePackTable;
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);

        public async Task<List<TilePack>> FindAll()
        {
            var list = new List<TilePack>();
            await _connection.OpenAsync();
            await using var command = new MySqlCommand(sql_select(Table), _connection);
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
                $"SELECT TilePack.Id, TilePack.Name, TilePack.PicUrl, {DbStrings.PriceStripe}, " +
                "CASE WHEN OwnedTilePack.OwnerId is Null THEN '0' ELSE '1' END as Owned " +
                "FROM TilePack " +
                $"LEFT JOIN OwnedTilePack ON OwnedTilePack.TilePackId = TilePack.Id && OwnedTilePack.OwnerId = '{userId}' ",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToEnt(reader);
                ent.IsOwned = Convert.ToBoolean(short.Parse(reader.GetString(4)));
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }
        
        public async Task<List<TilePack>> GetOwnedTilePacks(string userId)
        {
            var list = new List<TilePack>();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                "SELECT TilePack.Id AS TilePackId, TilePack.Name AS TilePackName, TilePack.PicUrl AS TilePackPic, TilePack.Stripe_PRICE AS TilePackStripe FROM `TilePack` JOIN OwnedTilePack ON TilePack.Id = OwnedTilePack.TilePackId WHERE OwnedTilePack.OwnerId = @ownerId;"
                , _connection);
            {
                command.Parameters.Add("@ownerId", MySqlDbType.VarChar).Value = userId;
            }
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(new TilePack(reader));

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
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);
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
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);
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
                $"VALUES ('{uuid}','{toCreate.Name}', '{toCreate.PicUrl ?? ""}', '{toCreate.PriceStripe ?? ""}'); " +
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{uuid}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);

            await _connection.CloseAsync();
            return ent ?? throw new Exception($"Error i creating {Table} with name: " + toCreate.Name);
        }

        public async Task Update(TilePack toUpdate)
        {
            await using var con = _connection;
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "UPDATE TilePack SET Name = @name, PicUrl = @picUrl, Stripe_PRICE = @stripe WHERE Id = @packId;",
                        con);
                {
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = toUpdate.Id;
                    command.Parameters.Add("@name", MySqlDbType.VarChar).Value = toUpdate.Name;
                    command.Parameters.Add("@picUrl", MySqlDbType.VarChar).Value = toUpdate.PicUrl;
                    command.Parameters.Add("@stripe", MySqlDbType.VarChar).Value = toUpdate.PriceStripe;
                }
                command.ExecuteNonQuery();
            }
        }

        public async Task Delete(string id)
        {
            await using var con = _connection;
            {
                con.Open();
                await using MySqlCommand command = new("DELETE FROM TilePack WHERE Id = @packId", con);
                {
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = id;
                }
                command.ExecuteNonQuery();
            }
        }

        private static TilePack ReaderToEnt(MySqlDataReader reader)
        {
            return new TilePack(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(),
                reader.GetValue(2).ToString(), reader.GetValue(3).ToString());
        }

        private static string sql_select(string from)
        {
            return $"SELECT * FROM {from} ";
        }
    }
}