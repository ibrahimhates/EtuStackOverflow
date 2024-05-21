using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Generic;

namespace AskForEtu.Core.Services.Repo
{
    public interface IPasswordResetRepository : IGenericRepository<PasswordReset,int>
    {
    }
}
