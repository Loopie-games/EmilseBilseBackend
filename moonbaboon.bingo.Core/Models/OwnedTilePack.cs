using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class OwnedTilePack
    {
        public OwnedTilePack(User owner, TilePack tilePack)
        {
            Owner = owner;
            TilePack = tilePack;
        }

        public OwnedTilePack(MySqlDataReader reader)
        {
            Owner = new User(reader);
            TilePack = new TilePack(reader);
        }

        public User Owner { get; set; }
        public TilePack TilePack { get; set; }
    }

    public class OwnedTilePackEntity
    {
        public OwnedTilePackEntity(string ownerId, string packId)
        {
            OwnerId = ownerId;
            PackId = packId;
        }

        public string OwnerId { get; set; }
        public string PackId { get; set; }
    }
}