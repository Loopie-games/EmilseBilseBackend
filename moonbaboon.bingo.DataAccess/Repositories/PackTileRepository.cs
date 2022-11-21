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
            _connection = connection;
        }

        public async Task<List<PackTile>> GetByPackId(string packId)
        {
            List<PackTile> list = new();
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        @"SELECT *
                        FROM PackTile 
                            JOIN Tile on PackTile.PackTile_TileId = Tile.Tile_Id 
                            JOIN TilePack on  TilePack_Id= PackTile.PackTile_PackId
                        WHERE PackTile.PackTile_PackId = @packId",
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
                        @"SELECT *
                        FROM PackTile 
                            JOIN Tile on PackTile.PackTile_TileId = Tile.Tile_Id 
                            JOIN TilePack on TilePack_Id = PackTile.PackTile_PackId 
                        WHERE PackTile.PackTile_Id = @Id",
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
                    new("INSERT INTO PackTile VALUES (@Id,@tileId,@packId);", con);
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
                    new("DELETE FROM PackTile WHERE PackTile_Id = @packId;", con);
                {
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = id;
                }
                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}