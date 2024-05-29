using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;
using Microsoft.EntityFrameworkCore;

namespace AskForEtu.Repository.Services.Repo;
public class RoleRepository : GenericRepository<Role, int>, IRoleRepository
{
    public RoleRepository(AskForEtuDbContext context) : base(context)
    {
    }

    public Task<Role> GetRoleByNameAsync(string roleName, bool trackChanges = false)
    {
        var role = GetByCondition(x => x.Name.Equals(roleName), trackChanges)
            .FirstOrDefaultAsync();
        
        return role;
    }
}
