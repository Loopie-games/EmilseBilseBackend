﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class LobbyRepository : ILobbyRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);
        private static Random random = new Random();

        public async Task<Lobby?> Create(Lobby lobbyToCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var pin = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DBStrings.LobbyTable}`(`{DBStrings.Id}`, `{DBStrings.Host}`, `{DBStrings.Pin}`) " +
                $"VALUES ('{uuid}','{lobbyToCreate.Host}','{pin}'); " +
                $"SELECT * FROM {DBStrings.LobbyTable} WHERE {DBStrings.LobbyTable}.{DBStrings.Id} = '{uuid}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetValue(1).ToString() != lobbyToCreate.Host) continue;
                lobbyToCreate.Id = reader.GetValue(0).ToString();
                lobbyToCreate.Pin = reader.GetValue(2).ToString();
            }
            
            await _connection.CloseAsync();

            if (lobbyToCreate.Id == null)
            {
                throw new InvalidDataException("ERROR: User not created");
            }
            return lobbyToCreate;
        }

        public async Task<LobbyForUser?> FindById(string id)
        {
            LobbyForUser? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.LobbyTable}.{DBStrings.Id}, host.{DBStrings.Nickname}, {DBStrings.LobbyTable}.{DBStrings.Pin} " +
                $"FROM {DBStrings.LobbyTable} " +
                $"JOIN {DBStrings.UserTable} as host " +
                $"ON host.{DBStrings.Id} = {DBStrings.LobbyTable}.{DBStrings.Host} " +
                $"WHERE {DBStrings.LobbyTable}.{DBStrings.Id} = '{id}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
                {
                    ent = new LobbyForUser(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(),  reader.GetValue(2).ToString());
                }
            }
            
            await _connection.CloseAsync();
            return ent;
        }
    }
}