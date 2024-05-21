using AskForEtu.Core.Entity;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.Context;
using AskForEtu.Repository.Services.Generic;

namespace AskForEtu.Repository.Services.Repo;
public class QuestionRepository : GenericRepository<Question, long>, IQuestionRepository
{
    public QuestionRepository(AskForEtuDbContext context) : base(context)
    {
    }
}
