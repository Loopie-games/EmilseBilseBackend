using System;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardMemberRepository: IBoardMemberRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BoardMemberRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public string Insert(BoardMemberEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();

            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText = @"Insert into BoardMember VALUES (@id, @UserId, @BoardId)";

            con.Open();
            
            var param1 = command.CreateParameter();
            param1.ParameterName = "@id";
            param1.Value = entity.Id;
            command.Parameters.Add(param1);

            var param2 = command.CreateParameter();
            param2.ParameterName = "@UserId";
            param2.Value = entity.UserId;
            command.Parameters.Add(param2);

            var param3 = command.CreateParameter();
            param3.ParameterName = "@BoardId";
            param3.Value = entity.BoardId;
            command.Parameters.Add(param3);
            
            
            command.ExecuteNonQuery();
            return entity.Id;
        }
    }
}