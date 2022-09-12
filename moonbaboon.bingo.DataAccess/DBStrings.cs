using System;

namespace moonbaboon.bingo.DataAccess
{
    public static class DbStrings
    {
        public const string SqlConnection =
            "Server=185.51.76.204; Database=emilse_bilse_bingo; Uid=root; PWD=hemmeligt;";

        //generally used
        public const string Id = "Id";

        //Friendship
        public const string FriendshipTable = "Friendship";

        //Rows
        public const string FriendId1 = "FriendId1";
        public const string FriendId2 = "FriendId2";
        public const string Accepted = "Accepted";

        //User
        public const string UserTable = "User";

        //Rows
        public const string Username = "Username";
        public const string Nickname = "Nickname";
        public const string Password = "Password";
        public const string Salt = "Salt";
        public const string ProfilePic = "ProfilePicURL";

        //Tile
        public const string TileTable = "Tile";

        //Rows
        public const string Action = "Action";

        //UserTile
        public const string UserTileTable = "UserTile";

        //Rows
        public const string UserId = "UserId";
        public const string AddedById = "AddedById";

        //Lobby
        public const string LobbyTable = "Lobby";

        //Rows
        public const string Host = "Host";
        public const string Pin = "Pin";

        //PendingPlayers
        public const string PendingPlayerTable = "PendingPlayer";

        //Rows
        public const string LobbyId = "LobbyId";

        //Game
        public const string GameTable = "Game";

        //Rows
        public const string WinnerId = "WinnerId";
        public const string HostId = "HostId";
        public const string State = "State";

        //Board 
        public const string BoardTable = "Board";

        //Rows
        public const string GameId = "GameId";

        //BoardTile
        public const string BoardTileTable = "BoardTile";
        //Rows
        public const string BoardId = "BoardId";
        public const string TileId = "TileId";
        public const string AboutUserId = "AboutUserId";
        public const string Position = "Position";
        public const string IsActivated = "IsActivated";
        public const string TileType = "TileType";
        
        //TopPlayer
        public const string TopPlayerTable = "TopPlayer";
        //Rows
        public const string TurnedTiles = "TurnedTiles";

        //TilePack
        public const string TilePackTable = "TilePack";
        public const string PicUrl = "PicUrl";

        //PackTile
        public const string PackTileTable = "PackTile";

        //Rows
        public const string PackId = "PackId";
        public const string Name = "Name";

        //OwnedTilePack
        public const string OwnedTilePackTable = "OwnedTilePack";

        //Rows
        public const string OwnerId = "OwnerId";
        public const string TilePackId = "TilePackId";

        //Admin
        public const string AdminTable = "Admin";
    }
}