using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;

namespace AskForEtu.Repository.Services.Repo
{
    public class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        public UserRepository(AskForEtuDbContext context) : base(context)
        {
        }
    }
}
