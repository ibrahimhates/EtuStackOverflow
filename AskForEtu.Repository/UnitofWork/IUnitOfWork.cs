
namespace AskForEtu.Repository.UnitofWork
{
    public interface IUnitOfWork
    {
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
