namespace moonbaboon.bingo.DataAccess
{
    public static class DatabaseStrings
    {
        public static string SqLconnection = "Server=185.51.76.204; Database=emilse_bilse_bingo; Uid=root; PWD=hemmeligt;";

        //generally used
        public const string Id = "Id";
        
        //booleans
        public const string False = "0";
        public const string True = "1";
        
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
        //Table
        public const string TileTable = "BingoTile";

        //Rows
        public const string UserId = "UserId";
        public const string Action = "Action";
        public const string AddedById = "AddedById";
    }
}