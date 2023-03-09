namespace PatrickParker.EmpowerID.Demo.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> Commit();
    }
}
