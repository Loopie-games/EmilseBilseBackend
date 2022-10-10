﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class OwnedTilePackRepository : IOwnedTilePackRepository
    {
        private const string Table = DbStrings.OwnedTilePackTable;
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);

        public async Task<List<OwnedTilePack>> GetOwnedTilePacks(string userId)
        {
            List<OwnedTilePack> list = new();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.OwnerId} = '{userId}'", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToEnt(reader);
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<bool> ConfirmOwnership(OwnedTilePackEntity ownedTp)
        {
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                @"SELECT 1 FROM OwnedTilePack WHERE OwnedTilePack.OwnerId = @ownerId && OwnedTilePack.TilePackId = @packId",
                _connection);
            {
                command.Parameters.Add("@ownerId", MySqlDbType.VarChar).Value = ownedTp.OwnerId;
                command.Parameters.Add("@packId", MySqlDbType.VarChar).Value = ownedTp.PackId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return Convert.ToBoolean(reader.GetByte(0));

            await _connection.CloseAsync();
            return false;
        }

        private static string sql_select(string from)
        {
            return
                $"SELECT U.{DbStrings.Id}, U.{DbStrings.Username}, U.{DbStrings.Nickname}, U.{DbStrings.ProfilePic}," +
                $"TP.{DbStrings.Id}, TP.{DbStrings.Name}, TP.{DbStrings.PicUrl}, TP.{DbStrings.PriceStripe} " +
                $"FROM {from} " +
                $"JOIN {DbStrings.UserTable} AS U ON {Table}.{DbStrings.OwnerId} = U.{DbStrings.Id} " +
                $"JOIN {DbStrings.TilePackTable} AS TP On {Table}.{DbStrings.TilePackId} = TP.{DbStrings.Id} ";
        }

        private static OwnedTilePack ReaderToEnt(MySqlDataReader reader)
        {
            UserSimple owner = new(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                reader.GetValue(3).ToString());
            TilePack tilePack = new(reader.GetString(4), reader.GetString(5), reader.GetValue(6).ToString(),
                reader.GetValue(7).ToString());
            return new OwnedTilePack(owner, tilePack);
        }
    }
}