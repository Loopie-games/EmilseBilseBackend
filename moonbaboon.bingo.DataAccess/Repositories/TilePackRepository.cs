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
        private readonly MySqlConnection _connection;

        public TilePackRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<TilePack>> FindAll()
        {
            var list = new List<TilePack>();
            await using var con = _connection.Clone();
            await using var command = new MySqlCommand(@"SELECT * FROM TilePack", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new TilePack(reader));
            }

            return list;
        }

        public async Task<List<TilePack>> FindAll_LoggedUser(string userId)
        {
            var list = new List<TilePack>();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT *, IF(OwnedTilePack.OwnedTilePack_OwnerId is Null, '0', '1') as Owned 
                FROM TilePack LEFT JOIN OwnedTilePack ON OwnedTilePack.OwnedTilePack_TilePackId = TilePack.TilePack_Id && OwnedTilePack.OwnedTilePack_OwnerId = @UserId ",
                con);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = new TilePack(reader)
                {
                    IsOwned = Convert.ToBoolean(short.Parse(reader.GetString(4)))
                };
                list.Add(ent);
            }

            return list;
        }

        public async Task<List<TilePack>> GetOwnedTilePacks(string userId)
        {
            var list = new List<TilePack>();
            await using var con = _connection.Clone();
            con.Open();
            await using MySqlCommand command = new(
                "SELECT * FROM TilePack JOIN OwnedTilePack OTP on TilePack.TilePack_Id = OTP.OwnedTilePack_TilePackId WHERE OTP.OwnedTilePack_OwnerId = @ownerId;"
                , con);
            {
                command.Parameters.Add("@ownerId", MySqlDbType.VarChar).Value = userId;
            }
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(new TilePack(reader));

            return list;
        }

        public async Task<TilePack> FindDefault()
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * FROM TilePack WHERE TilePack_Name = 'Default' "
                , con);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new TilePack(reader);
            }

            throw new Exception("no Default Package found");
        }

        public async Task<TilePack> FindById(string packId)
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * FROM TilePack WHERE TilePack_Id = @PackId "
                , con);
            {
                command.Parameters.Add("@PackId", MySqlDbType.VarChar).Value = packId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new TilePack(reader);
            }

            throw new Exception("no tilePackage found with id: " + packId);
        }

        public async Task<string> Create(TilePack entity)
        {
            var uuid = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            con.Open();
            await using MySqlCommand command = new(
                @"INSERT INTO TilePack VALUES (@Id,@Name, @PicUrl, @Stripe); "
                , con);
            {
                command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = uuid;
                command.Parameters.Add("@Name", MySqlDbType.VarChar).Value = entity.Name;
                command.Parameters.Add("@PicUrl", MySqlDbType.VarChar).Value = entity.PicUrl;
                command.Parameters.Add("@Stripe", MySqlDbType.VarChar).Value = entity.PriceStripe;
            }
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return uuid;
            }

            throw new Exception($"Error i creating {nameof(TilePack)} with name: " + entity.Name);
        }

        public async Task Update(TilePack entity)
        {
            await using var con = _connection;
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "UPDATE TilePack SET TilePack_Name = @name, TilePack_PicUrl = @picUrl, TilePack_Stripe_PRICE = @stripe WHERE TilePack_Id = @packId;",
                        con);
                {
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = entity.Id;
                    command.Parameters.Add("@name", MySqlDbType.VarChar).Value = entity.Name;
                    command.Parameters.Add("@picUrl", MySqlDbType.VarChar).Value = entity.PicUrl;
                    command.Parameters.Add("@stripe", MySqlDbType.VarChar).Value = entity.PriceStripe;
                }
                command.ExecuteNonQuery();
            }
        }

        public async Task Delete(string id)
        {
            await using var con = _connection;
            {
                con.Open();
                await using MySqlCommand command = new("DELETE FROM TilePack WHERE TilePack_Id = @packId", con);
                {
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = id;
                }
                command.ExecuteNonQuery();
            }
        }
    }
}