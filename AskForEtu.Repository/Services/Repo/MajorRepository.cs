
using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;

namespace AskForEtu.Repository.Services.Repo
{
    public class MajorRepository : GenericRepository<Major, byte>, IMajorRepository
    {
        public MajorRepository(AskForEtuDbContext context) : base(context)
        {
        }
    }
}
