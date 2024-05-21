
using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;

namespace AskForEtu.Repository.Services.Repo
{
    public class FacultyRepository : GenericRepository<Faculty, byte>, IFacultRepository
    {
        public FacultyRepository(AskForEtuDbContext context) : base(context)
        {
        }
    }
}
