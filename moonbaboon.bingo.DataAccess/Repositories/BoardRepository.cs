using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly MySqlConnection _connection;

        public BoardRepository(MySqlConnection connection)
        {
            _connection = connection;
        }


        public async Task<BoardEntity> FindById(string id)
        {
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        @"SELECT * 
                        FROM Board 
                        WHERE Board_Id = @Id",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) return new BoardEntity(reader);
            }

            throw new Exception($"No {nameof(BoardEntity)} with id: {id}");
        }

        public async Task<BoardEntity> Create(BoardEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "INSERT INTO Board VALUES (@Id, @GameId, @UserId)",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                    command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = entity.UserId;
                    command.Parameters.Add("@GameId", MySqlDbType.VarChar).Value = entity.GameId;
                }
                command.ExecuteNonQuery();
            }
            return entity;
        }

        public async Task<BoardEntity?> FindByUserAndGameId(string userId, string gameId)
        {
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command = new(
                    @"SELECT * 
                        FROM Board 
                        WHERE Board_UserId = @UserId AND Board_GameId = @GameId",
                    con);
                {
                    command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = userId;
                    command.Parameters.Add("@GameId", MySqlDbType.VarChar).Value = gameId;
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) return new BoardEntity(reader);
            }

            throw new Exception($"No {nameof(BoardEntity)} found");
        }

        public async Task<bool> IsBoardFilled(string boardId)
        {
            var b = false;
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command = new(
                    @"SELECT((SELECT COUNT(*) FROM BoardTile WHERE BoardTile_IsActivated = '1' AND BoardTile_BoardId = @Board_Id) = 24 IS true)",
                    con);
                {
                    command.Parameters.Add("@Board_Id", MySqlDbType.VarChar).Value = boardId;
                }
                await using MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync()) b = reader.GetBoolean(0);
            }
            return b;
        }

        public async Task<List<BoardEntity>> FindTopRanking(string gameId, int limit)
        {
            var list = new List<BoardEntity>();

            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command = new(
                    @"SELECT *, (SELECT COUNT(BoardTile_Id) FROM BoardTile WHERE BoardTile_IsActivated = '1' && BoardTile_BoardId= Board_Id) AS Board_TurnedTiles
                        FROM Board 
                            WHERE Board_GameId = @Game_Id
                            ORDER BY Board_TurnedTiles DESC 
                            LIMIT @Limit",
                    con);
                {
                    command.Parameters.Add("@Game_Id", MySqlDbType.VarChar).Value = gameId;
                    command.Parameters.Add("@Limit", MySqlDbType.Int24).Value = limit;
                }
                await using MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    list.Add(new BoardEntity(reader));
            }
            return list;
        }
    }
}