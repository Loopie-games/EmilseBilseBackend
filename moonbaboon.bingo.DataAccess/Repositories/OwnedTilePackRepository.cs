using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class OwnedTilePackRepository : IOwnedTilePackRepository
    {
        private readonly MySqlConnection _connection;

        public OwnedTilePackRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<OwnedTilePack>> GetOwnedTilePacks(string userId)
        {
            List<OwnedTilePack> list = new();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT * From OwnedTilePack 
JOIN User U on U.User_id = OwnedTilePack.OwnedTilePack_OwnerId
JOin TilePack TP on OwnedTilePack.OwnedTilePack_TilePackId = TP.TilePack_Id
WHERE OwnedTilePack_OwnerId = @UserId", con);
            {
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = userId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new OwnedTilePack(reader));
            }

            return list;
        }

        public async Task<bool> ConfirmOwnership(OwnedTilePackEntity ownedTp)
        {
            await using var con = _connection.Clone();

            await using var command = new MySqlCommand(
                @"SELECT 1 FROM OwnedTilePack WHERE OwnedTilePack_OwnerId= @ownerId && OwnedTilePack_TilePackId = @packId",
                _connection);
            {
                command.Parameters.Add("@ownerId", MySqlDbType.VarChar).Value = ownedTp.OwnerId;
                command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = ownedTp.PackId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return Convert.ToBoolean(reader.GetByte(0));

            return false;
        }
    }
}