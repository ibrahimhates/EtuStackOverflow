using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Generic;

namespace AskForEtu.Core.Services.Repo
{
    public interface IUserRepository : IGenericRepository<User,int>
    {
        Task<User> GetByUserOrEmailAsync(string userNameOrEmail, bool trackChanges = false);
        Task<User> GetByEmailVerifyTokenAsync(string token, bool trackChanges = false);
        Task<User> GetUserProfileDetail(int id, bool trackChanges = false);
    }
}
