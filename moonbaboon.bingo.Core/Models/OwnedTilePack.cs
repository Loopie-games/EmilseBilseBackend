namespace moonbaboon.bingo.Core.Models
{
    public class OwnedTilePack
    {
        public OwnedTilePack(UserSimple owner, TilePack tilePack)
        {
            Owner = owner;
            TilePack = tilePack;
        }

        public UserSimple Owner { get; set; }
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