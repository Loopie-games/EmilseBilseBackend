using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class PackTileRepository : IPackTileRepository
    {
        private const string Table = DbStrings.PackTileTable;
        private readonly MySqlConnection _connection;


        public PackTileRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<PackTile>> GetByPackId(string packId)
        {
            List<PackTile> list = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.PackId} = '{packId}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(ReaderToEnt(reader));

            await _connection.CloseAsync();
            return list;
        }

        public async Task<PackTile> Create(PackTile toCreate)
        {
            PackTile? ent = null;
            var uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"INSERT INTO {DbStrings.TileTable} " +
                $"VALUES ('{uuid}', '{toCreate.Action}');" +
                $"INSERT INTO {Table} " +
                $"VALUES ('{uuid}','{toCreate.Pack.Id}'); " +
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.TileId} = '{uuid}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);

            await _connection.CloseAsync();
            return ent ?? throw new Exception("Error i creating packtile with action: " + toCreate.Action);
        }

        public async Task<PackTile> GetById(string id)
        {
            PackTile? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.TileId} = '{id}'"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);

            await _connection.CloseAsync();
            return ent ?? throw new Exception("Error i creating packtile with Id: " + id);
        }

        public async Task<PackTile> GetPackTile(PackTileEntity pt)
        {
            await using var con = _connection;
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        "SELECT T.Id AS TileId, T.Action AS TileAction, TP.Id AS TilePackId, TP.Name AS TilePackName, TP.PicUrl AS TilePackPic, TP.Stripe_PRICE As TilePackPrice FROM PackTile JOIN Tile T on PackTile.TileId = T.Id JOIN TilePack TP on TP.Id = PackTile.PackId WHERE TileId = @tileId AND PackId=@packId",
                        con);
                {
                    command.Parameters.Add("@tileId", MySqlDbType.VarChar).Value = pt.TileId;
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = pt.PackId;
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                { 
                    return new PackTile(reader);
                }
                await con.CloseAsync();
            }

            throw new Exception("Error in " + nameof(GetPackTile));
        }

        public async Task<List<Tile>> GetTilesUsedInPacks()
        {
            var list = new List<Tile>();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                "SELECT * FROM Tile RIGHT JOIN PackTile on PackTile.TileId = Tile.Id"
                , _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(new Tile(reader.GetString(0), reader.GetString(1), null, TileType.PackTile));

            await _connection.CloseAsync();
            return list;
        }

        public async Task<PackTile> AddToPack(PackTileEntity pt)
        {
            await using var con = _connection;
            {
                con.Open();
                await using MySqlCommand command =
                    new("INSERT INTO PackTile(TileId, PackId) VALUES (@tileId,@packId);", con);
                {
                    command.Parameters.Add("@tileId", MySqlDbType.VarChar).Value = pt.TileId;
                    command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = pt.PackId;
                }
                command.ExecuteNonQuery();
                await con.CloseAsync();
            }
            return await GetPackTile(pt);
        }

        private static string sql_select(string from)
        {
            return
                $"SELECT T.{DbStrings.Id}, T.{DbStrings.Action}, TP.{DbStrings.Id}, TP.{DbStrings.Name}, TP.{DbStrings.PicUrl}, TP.{DbStrings.PriceStripe} " +
                $"FROM {from} " +
                $"JOIN {DbStrings.TileTable} AS T ON {Table}.{DbStrings.TileId} = T.{DbStrings.Id} " +
                $"JOIN {DbStrings.TilePackTable} AS TP On {Table}.{DbStrings.PackId} = TP.{DbStrings.Id} ";
        }

        private static PackTile ReaderToEnt(MySqlDataReader reader)
        {
            TilePack tilePack = new(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                reader.GetValue(4).ToString(), reader.GetValue(5).ToString());
            PackTile packTile =
                new(new Tile(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), null, TileType.PackTile),
                    tilePack);
            return packTile;
        }
    }
}