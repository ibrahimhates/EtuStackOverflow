using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;

namespace AskForEtu.Repository.Services.Repo
{
    public class TokenRepository : GenericRepository<Token, int>, ITokenRepository
    {
        public TokenRepository(AskForEtuDbContext context) : base(context)
        {
        }
    }
}
