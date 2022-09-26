using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class NewTileDto
    {
        public NewTileDto(string action, string aboutUserId)
        {
            Action = action;
            AboutUserId = aboutUserId;
        }

        [Required] public string Action { get; set; }

        [Required] public string AboutUserId { get; set; }
    }

    public class NewPackTileDto
    {
        public NewPackTileDto(string action, string packId)
        {
            Action = action;
            PackId = packId;
        }

        [Required] public string Action { get; set; }

        [Required] public string PackId { get; set; }
    }

    public class NewTilePackDto
    {
        public NewTilePackDto(string name, string? picUrl)
        {
            Name = name;
            PicUrl = picUrl;
        }

        [Required] public string Name { get; set; }

        public string? PicUrl { get; set; }
    }

    public class TilePackDto
    {
        [JsonConstructor]
        public TilePackDto(string name)
        {
            Name = name;
        }
        public TilePackDto(TilePack tp)
        {
            Id = tp.Id;
            Name = tp.Name;
            PicUrl = tp.PicUrl;
            IsOwned = tp.IsOwned;
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public string? PicUrl { get; set; }
        public bool? IsOwned { get; set; }
        public long? Price { get; set; }

        public TilePack ToTilePack()
        {
            return new TilePack(Id, Name, PicUrl, null);
        }
    }
}