namespace moonbaboon.bingo.Core.Models
{
    public class Admin : UserSimple
    {
        public Admin(string? adminId, UserSimple userSimple) : base(userSimple.Id, userSimple.Username,
            userSimple.Nickname, userSimple.ProfilePicUrl)
        {
            AdminId = adminId;
            Id = userSimple.Id;
            Username = userSimple.Username;
            Nickname = userSimple.Nickname;
            ProfilePicUrl = userSimple.ProfilePicUrl;
        }

        public string? AdminId { get; set; }
        public string? Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string? ProfilePicUrl { get; set; }
    }
}