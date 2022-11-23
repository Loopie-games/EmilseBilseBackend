using System.Data;

namespace moonbaboon.bingo.Core.Models
{
    public class BoardMemberEntity
    {
        public BoardMemberEntity(string? id, string userId, string boardId)
        {
            Id = id;
            UserId = userId;
            BoardId = boardId;
        }

        public string? Id { get; set; }
        public string UserId { get; set; }
        public string BoardId { get; set; }
    }

    public class BoardMember
    {
        public BoardMember(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("BoardMember_Id"));
            User = new User(reader);
            Board = new BoardEntity(reader);
        }

        public string Id { get; set; }
        public User User { get; set; }
        public BoardEntity Board { get; set; }
    }
}