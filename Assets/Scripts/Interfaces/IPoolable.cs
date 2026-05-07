namespace CoreBreach.Interfaces
{
    public interface IPoolable
    {
        void OnTakenFromPool();
        void OnReturnedToPool();
    }
}
