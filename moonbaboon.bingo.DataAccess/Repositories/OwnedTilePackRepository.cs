using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class OwnedTilePackRepository : IOwnedTilePackRepository
    {
        private readonly MySqlConnection _connection;
        private readonly IDbConnectionFactory _connectionFactory;

        public OwnedTilePackRepository(MySqlConnection connection, IDbConnectionFactory connectionFactory)
        {
            _connection = connection;
            _connectionFactory = connectionFactory;
        }

        public List<OwnedTilePack> GetOwnedTilePacks(string userId)
        {
            List<OwnedTilePack> list = new();
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText =
                @"SELECT * From OwnedTilePack 
JOIN User U on U.User_id = OwnedTilePack.OwnedTilePack_OwnerId
JOin TilePack TP on OwnedTilePack.OwnedTilePack_TilePackId = TP.TilePack_Id
WHERE OwnedTilePack_OwnerId = @UserId";

            var param = command.CreateParameter();
            param.ParameterName = "@UserId";
            param.Value = userId;
            command.Parameters.Add(param);

            con.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) list.Add(new OwnedTilePack(reader));

            return list;
        }

        public bool ConfirmOwnership(OwnedTilePackEntity ownedTp)
        {
            using var con = _connectionFactory.CreateConnection();

            using var command = con.CreateCommand();
            command.CommandText = @"SELECT 1 FROM OwnedTilePack WHERE OwnedTilePack_OwnerId= @ownerId && OwnedTilePack_TilePackId = @packId";
            var param1 = command.CreateParameter();
            param1.ParameterName = "@ownerId";
            param1.Value = ownedTp.OwnerId;
            command.Parameters.Add(param1);
            
            var param2 = command.CreateParameter();
            param2.ParameterName = "@packId";
            param2.Value = ownedTp.PackId;
            command.Parameters.Add(param2);
            
            con.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) return Convert.ToBoolean(reader.GetByte(0));

            return false;
        }
    }
}