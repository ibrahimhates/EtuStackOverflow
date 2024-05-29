using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Generic;

namespace AskForEtu.Core.Services.Repo
{
    public interface IRoleRepository : IGenericRepository<Role,int>
    {
        Task<Role> GetRoleByNameAsync(string roleName, bool trackChanges = false);
    }
}
