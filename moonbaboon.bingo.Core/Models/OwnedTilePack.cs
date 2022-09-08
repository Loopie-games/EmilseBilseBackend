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
}