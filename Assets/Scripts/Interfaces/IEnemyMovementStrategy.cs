namespace CoreBreach.Interfaces
{
    public interface IEnemyMovementStrategy
    {
        void Move(Enemies.EnemyUnit enemy, float deltaTime);
    }
}
