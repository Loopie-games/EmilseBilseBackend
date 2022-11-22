using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IAuthService
    {
        public string EncodeJwt(User user, byte[] tokenKey);

        public string Create(AuthEntity entity);
    }
}