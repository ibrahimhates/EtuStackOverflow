using AskForEtu.Repository.Context;

namespace AskForEtu.Repository.UnitofWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AskForEtuDbContext _context;

        public UnitOfWork(AskForEtuDbContext context)
        {
            _context=context;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default) 
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
