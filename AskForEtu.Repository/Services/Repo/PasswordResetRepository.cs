using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;

namespace AskForEtu.Repository.Services.Repo
{
    public class PasswordResetRepository : GenericRepository<PasswordReset, int>, IPasswordResetRepository
    {
        public PasswordResetRepository(AskForEtuDbContext context) : base(context)
        {
        }
    }
}
