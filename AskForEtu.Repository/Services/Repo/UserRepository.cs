using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AskForEtu.Repository.Services.Repo
{
    public class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        public UserRepository(AskForEtuDbContext context) : base(context)
        {
        }

        public async Task<User> GetByUserOrEmailAsync(string userNameOrEmail, bool trackChanges = false)
        {
            var user = await GetByCondition(x => x.Email == userNameOrEmail
            || x.UserName == userNameOrEmail, trackChanges).FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> GetByEmailVerifyTokenAsync(string token, bool trackChanges = false)
        {
            var user = await GetByCondition(x => x.VerifyEmailToken.Equals(token) 
                ,trackChanges).FirstOrDefaultAsync();

            return user;
        }

        public string GenerateEmailVerifyToken()
        {
            var tokenBytes = GenerateRandomBytes(32);

            string token = Convert.ToBase64String(tokenBytes);

            return token;
        }

        private byte[] GenerateRandomBytes(int lenght)
        {
            var randomBytes = new byte[lenght];

            using var rndg = RandomNumberGenerator.Create();

            rndg.GetBytes(randomBytes);

            return randomBytes;
        }
    }
}
