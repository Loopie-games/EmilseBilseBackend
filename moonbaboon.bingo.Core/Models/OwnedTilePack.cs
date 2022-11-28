using System.Data;

namespace moonbaboon.bingo.Core.Models
{
    public class OwnedTilePack
    {
        public OwnedTilePack(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("OwnedTilePack_Id"));
            Owner = new User(reader);
            TilePack = new TilePack(reader);
        }

        public string Id { get; set; }
        public User Owner { get; set; }
        public TilePack TilePack { get; set; }
    }

    public class OwnedTilePackEntity
    {
        public OwnedTilePackEntity(string? id, string ownerId, string packId)
        {
            Id = id;
            OwnerId = ownerId;
            PackId = packId;
        }

        public string? Id { get; set; }
        public string OwnerId { get; set; }
        public string PackId { get; set; }
    }
}