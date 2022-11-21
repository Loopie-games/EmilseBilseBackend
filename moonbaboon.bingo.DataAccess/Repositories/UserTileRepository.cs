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
            _connection = connection.Clone();
        }

        public async Task<List<UserTile>> FindAll()
        {
            await using var con = _connection.Clone();
            List<UserTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile.Id As UserTile_ID, 
       Tile.Id As Tile_Id, Tile.Action As Tile_Action, 
       About.id As About_Id, About.username As About_Username, About.nickname As About_Nickname, About.ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.id As AddedBy_Id, AddedBy.username As AddedBy_Username, AddedBy.nickname As AddedBy_Nickname, AddedBy.ProfilePicURL As AddedBy_ProfilePicUrl 
FROM UserTile 
    JOIN Tile ON UserTile.TileId = Tile.Id 
    JOIN User As About ON UserTile.AboutUserId = About.id 
    JOIN User As AddedBy ON UserTile.AddedByUser = AddedBy.id;"
                , con);

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new UserTile(reader));

            await con.CloseAsync();
            return list;
        }

        public async Task<UserTile?> FindById(string id)
        {
            await using var con = _connection.Clone();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile.Id As UserTile_ID, 
       Tile.Id As Tile_Id, Tile.Action As Tile_Action, 
       About.id As About_Id, About.username As About_Username, About.nickname As About_Nickname, About.ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.id As AddedBy_Id, AddedBy.username As AddedBy_Username, AddedBy.nickname As AddedBy_Nickname, AddedBy.ProfilePicURL As AddedBy_ProfilePicUrl 
FROM UserTile 
    JOIN Tile ON UserTile.TileId = Tile.Id 
    JOIN User As About ON UserTile.AboutUserId = About.id 
    JOIN User As AddedBy ON UserTile.AddedByUser = AddedBy.id
WHERE UserTile.Id = @Id;"
                , con);
            command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return new UserTile(reader);

            await con.CloseAsync();
            throw new Exception($"no {nameof(UserTile)} with id: " + id);
        }

        public async Task<List<UserTile>> GetAboutUserById(string id)
        {
            await using var con = _connection.Clone();
            List<UserTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile.Id As UserTile_ID, 
       Tile.Id As Tile_Id, Tile.Action As Tile_Action, 
       About.id As About_Id, About.username As About_Username, About.nickname As About_Nickname, About.ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.id As AddedBy_Id, AddedBy.username As AddedBy_Username, AddedBy.nickname As AddedBy_Nickname, AddedBy.ProfilePicURL As AddedBy_ProfilePicUrl 
FROM UserTile 
    JOIN Tile ON UserTile.TileId = Tile.Id 
    JOIN User As About ON UserTile.AboutUserId = About.id 
    JOIN User As AddedBy ON UserTile.AddedByUser = AddedBy.id
WHERE UserTile.AboutUserId = @Id;"
                , con);
            command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
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
                        "INSERT INTO UserTile(Id, TileId, AboutUserId, AddedByUser) VALUES (@Id, @TileId, @AboutUserId, @AddedByUserId);",
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
                @"SELECT UserTile.Id As UserTile_ID, 
       Tile.Id As Tile_Id, Tile.Action As Tile_Action, 
       About.id As About_Id, About.username As About_Username, About.nickname As About_Nickname, About.ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.id As AddedBy_Id, AddedBy.username As AddedBy_Username, AddedBy.nickname As AddedBy_Nickname, AddedBy.ProfilePicURL As AddedBy_ProfilePicUrl 
FROM (SELECT PendingPlayer.UserId As uId
      FROM PendingPlayer WHERE PendingPlayer.LobbyId = @lobbyId
      AND PendingPlayer.UserId != @userId) As pp
      JOIN UserTile ON pp.uid = UserTile.AboutUserId
      JOIN Tile On Tile.Id = UserTile.TileId
      JOIN User As About ON UserTile.AboutUserId = About.id 
    JOIN User As AddedBy ON UserTile.AddedByUser = AddedBy.id"
                , con);
            command.Parameters.Add("@lobbyId", MySqlDbType.VarChar).Value = lobbyId;
            command.Parameters.Add("@userId", MySqlDbType.VarChar).Value = userId;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new UserTile(reader));

            await con.CloseAsync();
            return list;
        }

        public async Task<List<UserTile>> FindMadeByUserId(string userId)
        {
            await using var con = _connection.Clone();
            List<UserTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT UserTile.Id As UserTile_ID, 
       Tile.Id As Tile_Id, Tile.Action As Tile_Action, 
       About.id As About_Id, About.username As About_Username, About.nickname As About_Nickname, About.ProfilePicURL As About_ProfilePicUrl, 
       AddedBy.id As AddedBy_Id, AddedBy.username As AddedBy_Username, AddedBy.nickname As AddedBy_Nickname, AddedBy.ProfilePicURL As AddedBy_ProfilePicUrl 
FROM UserTile 
    JOIN Tile ON UserTile.TileId = Tile.Id 
    JOIN User As About ON UserTile.AboutUserId = About.id 
    JOIN User As AddedBy ON UserTile.AddedByUser = AddedBy.id
WHERE UserTile.AddedByUser = @Id;"
                , con);
            command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = userId;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new UserTile(reader));

            await con.CloseAsync();
            return list;
        }
    }
}