namespace CoreBreach.Interfaces
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(float amount);
    }
}
