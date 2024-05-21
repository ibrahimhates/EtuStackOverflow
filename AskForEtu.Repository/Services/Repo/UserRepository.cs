using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
            || x.UserName == userNameOrEmail, trackChanges)
                .Include(x => x.Token)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> GetByEmailVerifyTokenAsync(string token, bool trackChanges = false)
        {
            var user = await GetByCondition(x => x.VerifyEmailToken.Equals(token)
                , trackChanges).FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> GetUserProfileDetail(int id, bool trackChanges = false)
        {
            var user = await GetByCondition(x => x.Id == id, trackChanges)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync();

            return user;
        }
    }
}
