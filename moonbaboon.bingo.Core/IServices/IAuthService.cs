using Microsoft.IdentityModel.Tokens;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IAuthService
    {
        public string EncodeJwt(UserSimple user, byte[] tokenKey);
    }
}