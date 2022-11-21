using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class PackTileRepository : IPackTileRepository
    {
        private readonly MySqlConnection _connection;

        public PackTileRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }

        public async Task<List<PackTile>> GetByPackId(string packId)
        {
            List<PackTile> list = new();
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        @"SELECT PackTile.Id As PackTile_Id, T.Id AS Tile_Id, T.Action AS Tile_Action, 
                        TP.Id AS TilePack_Id, TP.Name AS TilePack_Name, TP.PicUrl AS TilePack_Pic, TP.Stripe_PRICE As TilePack_Stripe 
                        FROM PackTile 
                            JOIN Tile T on PackTile.TileId = T.Id 
                            JOIN TilePack TP on TP.Id = PackTile.PackId 
                        WHERE PackTile.PackId = @packId",
                        con);
                {
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = packId;
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) list.Add(new PackTile(reader));
            }
            return list;
        }

        public async Task<PackTile> GetById(string id)
        {
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        @"SELECT PackTile.Id As PackTileId, T.Id AS TileId, T.Action AS TileAction, 
                        TP.Id AS TilePackId, TP.Name AS TilePackName, TP.PicUrl AS TilePackPic, TP.Stripe_PRICE As TilePackPrice 
                        FROM PackTile 
                            JOIN Tile T on PackTile.TileId = T.Id 
                            JOIN TilePack TP on TP.Id = PackTile.PackId 
                        WHERE PackTile.Id = @Id",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) return new PackTile(reader);
            }

            throw new Exception($"No {nameof(PackTile)} with id: {id}");
        }

        public async Task<PackTileEntity> Create(PackTileEntity pt)
        {
            pt.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new("INSERT INTO PackTile(Id, TileId, PackId) VALUES (@Id,@tileId,@packId);", con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = pt.Id;
                    command.Parameters.Add("@tileId", MySqlDbType.VarChar).Value = pt.TileId;
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = pt.PackId;
                }
                command.ExecuteNonQuery();
            }
            return pt;
        }

        public async Task<bool> Clear(string id)
        {
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new("DELETE FROM PackTile WHERE PackId = @packId;", con);
                {
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = id;
                }
                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}