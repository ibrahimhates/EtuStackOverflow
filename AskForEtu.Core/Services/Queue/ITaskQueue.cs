namespace AskForEtu.Core.Services.Queue
{
    public interface ITaskQueue<T>
    {
        Task AddQueue(T task);
        Task<T> DeQueue(CancellationToken cancellationToken);
    }
}
