using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class UserTileRepository : IUserTileRepository
    {
        private readonly MySqlConnection _connection;


        public UserTileRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<UserTile>> FindAll()
        {
            await using var con = _connection.Clone();
            List<UserTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile_ID, 
       Tile_Id, Tile_Action, 
       About.User_id As About_Id, About.User_Username As About_Username, About.User_Nickname As About_Nickname, About.User_ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.User_id As AddedBy_Id, AddedBy.User_Username As AddedBy_Username, AddedBy.User_Nickname As AddedBy_Nickname, AddedBy.User_ProfilePicURL As AddedBy_ProfilePicUrl 
FROM UserTile 
    JOIN Tile ON UserTile.UserTile_TileId = Tile_Id
    JOIN User As About ON UserTile.UserTile_About_UserId= About.User_id
    JOIN User As AddedBy ON UserTile.UserTile_AddedBy_UserId = AddedBy.User_id;"
                , con);

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new UserTile(reader));
            return list;
        }

        public async Task<UserTile> FindById(string id)
        {
            await using var con = _connection.Clone();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile_ID, 
       Tile_Id, Tile_Action, 
       About.User_id As About_Id, About.User_Username As About_Username, About.User_Nickname As About_Nickname, About.User_ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.User_id As AddedBy_Id, AddedBy.User_Username As AddedBy_Username, AddedBy.User_Nickname As AddedBy_Nickname, AddedBy.User_ProfilePicURL As AddedBy_ProfilePicUrl 
FROM UserTile 
    JOIN Tile ON UserTile.UserTile_TileId = Tile_Id
    JOIN User As About ON UserTile.UserTile_About_UserId= About.User_id
    JOIN User As AddedBy ON UserTile.UserTile_AddedBy_UserId = AddedBy.User_id
WHERE UserTile.UserTile_Id = @Id;"
                , con);
            command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return new UserTile(reader);
            
            throw new Exception($"no {nameof(UserTile)} with id: " + id);
        }

        public async Task<List<UserTile>> GetAboutUserById(string aboutUserId)
        {
            await using var con = _connection.Clone();
            List<UserTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile_ID, 
       Tile_Id, Tile_Action, 
       About.User_id As About_Id, About.User_Username As About_Username, About.User_Nickname As About_Nickname, About.User_ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.User_id As AddedBy_Id, AddedBy.User_Username As AddedBy_Username, AddedBy.User_Nickname As AddedBy_Nickname, AddedBy.User_ProfilePicURL As AddedBy_ProfilePicUrl 
FROM UserTile 
    JOIN Tile ON UserTile.UserTile_TileId = Tile_Id
    JOIN User As About ON UserTile.UserTile_About_UserId= About.User_id
    JOIN User As AddedBy ON UserTile.UserTile_AddedBy_UserId = AddedBy.User_id
WHERE UserTile.UserTile_About_UserId = @Id;"
                , con);
            command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = aboutUserId;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new UserTile(reader));

            await con.CloseAsync();
            return list;
        }

        public async Task<UserTileEntity> Create(UserTileEntity toCreate)
        {
            toCreate.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "INSERT INTO UserTile VALUES (@Id, @TileId, @AboutUserId, @AddedByUserId);",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = toCreate.Id;
                    command.Parameters.Add("@AboutUserId", MySqlDbType.VarChar).Value = toCreate.AboutUserId;
                    command.Parameters.Add("@TileId", MySqlDbType.VarChar).Value = toCreate.TileId;
                    command.Parameters.Add("@AddedByUserId", MySqlDbType.VarChar).Value = toCreate.AddedByUserId;
                }
                command.ExecuteNonQuery();
            }
            return toCreate;
        }

        public async Task<List<UserTile>> GetTilesForBoard(string lobbyId, string userId)
        {
            await using var con = _connection.Clone();
            List<UserTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile_ID, 
       Tile_Id, Tile_Action, 
       About.User_id As About_Id, About.User_Username As About_Username, About.User_Nickname As About_Nickname, About.User_ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.User_id As AddedBy_Id, AddedBy.User_Username As AddedBy_Username, AddedBy.User_Nickname As AddedBy_Nickname, AddedBy.User_ProfilePicURL As AddedBy_ProfilePicUrl 
FROM (SELECT PendingPlayer.PendingPlayer_UserId As uId
      FROM PendingPlayer WHERE PendingPlayer.PendingPlayer_LobbyId = @lobbyId
      AND PendingPlayer.PendingPlayer_UserId!= @userId) As pp
      JOIN UserTile ON pp.uid = UserTile.UserTile_About_UserId
      JOIN Tile On Tile.Tile_Id = UserTile.UserTile_TileId
      JOIN User As About ON UserTile.UserTile_About_UserId = About.User_id
    JOIN User As AddedBy ON UserTile.UserTile_AddedBy_UserId = AddedBy.User_id"
                , con);
            command.Parameters.Add("@lobbyId", MySqlDbType.VarChar).Value = lobbyId;
            command.Parameters.Add("@userId", MySqlDbType.VarChar).Value = userId;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new UserTile(reader));
            
            return list;
        }

        public async Task<List<UserTile>> FindAddedByUserId(string userId)
        {
            await using var con = _connection.Clone();
            List<UserTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile_ID, 
       Tile_Id, Tile_Action, 
       About.User_id As About_Id, About.User_Username As About_Username, About.User_Nickname As About_Nickname, About.User_ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.User_id As AddedBy_Id, AddedBy.User_Username As AddedBy_Username, AddedBy.User_Nickname As AddedBy_Nickname, AddedBy.User_ProfilePicURL As AddedBy_ProfilePicUrl  
FROM UserTile 
    JOIN Tile ON UserTile.UserTile_TileId = Tile.Tile_Id
    JOIN User As About ON UserTile.UserTile_About_UserId = About.User_id
    JOIN User As AddedBy ON UserTile_AddedBy_UserId = AddedBy.User_id
WHERE UserTile.UserTile_AddedBy_UserId = @Id;"
                , con);
            command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = userId;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new UserTile(reader));

            await con.CloseAsync();
            return list;
        }
    }
}