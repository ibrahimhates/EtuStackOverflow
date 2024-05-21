using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;

namespace AskForEtu.Repository.Services.Repo
{
    public class CommentRepository : GenericRepository<Comment, long>, ICommentRepository
    {
        public CommentRepository(AskForEtuDbContext context) : base(context)
        {
        }
    }
}
