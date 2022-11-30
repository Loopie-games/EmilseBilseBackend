using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly MySqlConnection _connection;
        private readonly IDbConnectionFactory _connectionFactory;

        public BoardRepository(MySqlConnection connection, IDbConnectionFactory connectionFactory)
        {
            _connection = connection;
            _connectionFactory = connectionFactory;
        }


        public async Task<Board> FindById(string id)
        {
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        @"SELECT * 
                        FROM Board 
JOIN Game G on G.Game_Id = Board.Board_GameId
JOIN User U on U.User_id = G.Game_HostId
                        WHERE Board_Id = @Id
                       ",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) return new Board(reader);
            }

            throw new Exception($"No {nameof(BoardEntity)} with id: {id}");
        }

        public async Task<string> Create(BoardEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "INSERT INTO Board VALUES (@Id, @GameId)",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                    command.Parameters.Add("@GameId", MySqlDbType.VarChar).Value = entity.GameId;
                }
                command.ExecuteNonQuery();
            }
            return entity.Id;
        }

        public BoardEntity FindByUserAndGameId(string userId, string gameId)
        {
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText = @"Select * from Board 
Join BoardMember BM on Board.Board_Id = BM.BoardMember_BoardId
WHERE Board_GameId = @gameId AND BoardMember_UserId = @userId";

            var param1 = command.CreateParameter();
            param1.ParameterName = "@gameId";
            param1.Value = gameId;
            command.Parameters.Add(param1);

            var param2 = command.CreateParameter();
            param2.ParameterName = "@userId";
            param2.Value = userId;
            command.Parameters.Add(param2);
            con.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) return new BoardEntity(reader);

            throw new Exception($"No {nameof(BoardEntity)} found from userid: {userId} and gameId {gameId}");
        }

        public async Task<bool> IsBoardFilled(string boardId)
        {
            var b = false;
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command = new(
                    @"SELECT((SELECT COUNT(*) FROM BoardTile WHERE BoardTile_ActivatedBy Is not null AND BoardTile_BoardId = @Board_Id) = 24 IS true)",
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
                    @"SELECT *, (SELECT COUNT(BoardTile_Id) FROM BoardTile WHERE BoardTile_ActivatedBy Is not null && BoardTile_BoardId= Board_Id) AS Board_TurnedTiles
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