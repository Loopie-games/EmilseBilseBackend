using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;

        public AuthService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public string EncodeJwt(User user, byte[] tokenKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor;
            var admin = _adminRepository.IsAdmin(user).Result;
            if (admin is not null)
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.Name, user.Username),
                        new(ClaimTypes.NameIdentifier, user.Id!, "userId"),
                        new(ClaimTypes.Role, nameof(Admin))
                    }),
                    Expires = DateTime.UtcNow.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };
            }
            else
            {
              tokenDescriptor = new SecurityTokenDescriptor
                          {
                              Subject = new ClaimsIdentity(new Claim[]
                              {
                                  new Claim(ClaimTypes.Name, user.Username),
                                  new Claim(ClaimTypes.NameIdentifier, user.Id!, "userId"),
                              }),
                              Expires = DateTime.UtcNow.AddDays(30),
                              SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                                  SecurityAlgorithms.HmacSha256Signature)
                          };  
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        
    }
}